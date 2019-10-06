using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using OhKeyCapsTester.Contracts;
using OhKeyCapsTester.Contracts.Messages;
using OhKeyCapsTester.MessageBusService;

namespace OhKeyCapsTester.Core.Hid
{
    public interface IHidWatcher : IService
    {
        Task Watch(CancellationToken token);
    }
    public class HidWatcher : IHidWatcher
    {
        private const int ColMask =     0b00000111;
        private const int RowMask =     0b01111000;
        private const int PressedMask = 0b10000000;
        private const int UserDefinedUsagePage = 0xFF31;
        private const int UsageKeyboardExecute = 0x0074;

        private readonly IMessageBus _bus;
        private readonly IHidNative _hid;

        public HidWatcher(IMessageBus bus, IHidNative hid)
        {
            _bus = bus;
            _hid = hid;
        }
        public Task Watch(CancellationToken token)
        {
            var hid = new SafeFileHandle(IntPtr.Zero, false);
            try
            {
                while (true)
                {
                    if (hid.IsInvalid)
                    {
                        hid = _hid.Open(0, 0, UserDefinedUsagePage, UsageKeyboardExecute);
                        if(!hid.IsInvalid)
                            _bus.Publish<DeviceFound>();
                    }
                    else
                    {
                        CheckHidForMessage(hid);
                    }
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(1000);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _hid.Close(hid);
            }
            return Task.CompletedTask;
        }

        private void CheckHidForMessage(SafeFileHandle hid)
        {
            var bigBuffer = new List<byte>(150);
            using (var stream = new FileStream(hid, FileAccess.Read))
            {
                var read = 0;
                do
                {
                    var buffer = new byte[100];
                    read = stream.Read(buffer, 0, buffer.Length);

                    for (var i = 0; i < read; i++)
                    {
                        var item = buffer[i];
                        if(item > 0)
                            bigBuffer.Add(item);
                    }

                    if ((bigBuffer.Count % 2) == 0)
                    {
                        ParseMessage(bigBuffer.ToArray());
                        bigBuffer.Clear();
                    }
                } while (read > 0);
            }
        }

        private void ParseMessage(byte[] buffer)
        {
            var a = Encoding.ASCII.GetString(buffer);
            for(var i = 0; i <a.Length; i+=2)
            {
                var code = ((GetHexVal(a[i])) << 4) + (GetHexVal(a[i+1]));
                var rec = new KeyEvent(
                    row: (code & RowMask) >> 3,
                    col: (code & ColMask),
                    pressed:(code & PressedMask) >> 7
                );
                _bus.Publish(rec);
            }
        }

        private static int GetHexVal(char hex) => hex - (hex < 58 ? 48 : 55);
    }
}
