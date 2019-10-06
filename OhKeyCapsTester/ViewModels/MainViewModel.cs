using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OhKeyCapsTester.Contracts;
using OhKeyCapsTester.Contracts.Messages;
using OhKeyCapsTester.Core;
using OhKeyCapsTester.Core.Hid;
using OhKeyCapsTester.MessageBusService;

namespace OhKeyCapsTester.ViewModels
{
    public class KeyPressedEvent : NotifyPropertyChanged
    {
        private int? _pressed;
        private int _pressedCount;

        public KeyPressedEvent(string keyName, int index)
        {
            KeyName = keyName;
            Index = index;
        }

        public int? Pressed
        {
            get => _pressed;
            set => Set(ref _pressed, value);
        }

        public int PressedCount
        {
            get => _pressedCount;
            set => Set(ref _pressedCount, value);
        }
        public string KeyName { get; }
        public int Index { get; }
    }
    public class MainViewModel : ViewModelBase, IMainWindow, IHandle<WindowLoaded>, IHandle<WindowClosing>, IHandle<KeyEvent>, IHandle<DeviceFound>
    {
        public int Rows => 10;
        public int Cols => 5;
        private readonly Lazy<int> HalfRows;
        private readonly IHidWatcher _hidWatcher;
        private string _state;
        public IDictionary<string, KeyPressedEvent> Events { get; }
        public string State { get => _state; set => Set(ref _state, value); }

        private CancellationTokenSource _cancellationTokenSource;

        public MainViewModel(IMessageBus messageBus, IHidWatcher hidWatcher) : base(messageBus)
        {
            HalfRows = new Lazy<int>(() => Rows/2);
            _hidWatcher = hidWatcher;
            MessageBus.Subscribe(this);
            var pressedEvents = new List<KeyPressedEvent>();
            for(var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
            {
                var keyName = CreateKeyName(r, c);
                pressedEvents.Add(new KeyPressedEvent(keyName, pressedEvents.Count));
            }
            Events = pressedEvents.ToDictionary(x => x.KeyName);
        }

        private string CreateKeyName(int r, int c)
        {
            var r2 = HalfRows.Value;
            var isRightHand = r >= r2;
            var lr = isRightHand ? "R" : "L";
            var ar = isRightHand ? r - r2 : r;
            return $"{lr}{ar}{c}";
        }

        public void Handle(WindowLoaded message)
        {
            State = "Loaded";
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => _hidWatcher.Watch(_cancellationTokenSource.Token));
        }

        public void Handle(WindowClosing message)
        {
            _cancellationTokenSource.Cancel();
        }

        public void Handle(KeyEvent message)
        {
            var index = CreateKeyName(message.Row, message.Col);
            Events[index].PressedCount += message.Pressed;
            Events[index].Pressed = message.Pressed;
            State = $"KeyEvent for r:{message.Row} c:{message.Col}";
        }

        public void Handle(DeviceFound message)
        {
            State = "Listening for device";
        }
    }
}
