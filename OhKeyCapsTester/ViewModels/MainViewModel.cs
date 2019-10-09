using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
        private int _x;
        private int _y;

        public KeyPressedEvent(string keyName)
        {
            KeyName = keyName;
        }

        public int? Pressed { get => _pressed; set => Set(ref _pressed, value); }

        public int PressedCount { get => _pressedCount; set => Set(ref _pressedCount, value); }
        public string KeyName { get; }
        public bool IsUsed { get; set; }

        public int X { get => _x; set => Set(ref _x, value); }
        public int Y { get => _y; set => Set(ref _y, value); }
    }
    public class MainViewModel : ViewModelBase, IMainWindow, IHandle<WindowLoaded>, IHandle<WindowClosing>, IHandle<KeyEvent>, IHandle<DeviceFound>
    {
        private const int SizeOffset = 60; // probably not the right place for this... but i dont know how to multiply in mvvm
        private string _state;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableKeyedCollection<string, KeyPressedEvent> _events;
        private ObservableKeyedCollection<string, KeyPressedEvent> _notUsedKeys;

        private readonly Lazy<int> HalfRows;
        private readonly IHidWatcher _hidWatcher;
        private readonly IKeyboardReaderService _keyboardReaderService;

        public MainViewModel(IMessageBus messageBus, IHidWatcher hidWatcher, IKeyboardReaderService keyboardReaderService) : base(messageBus)
        {
            HalfRows = new Lazy<int>(() => Rows/2);
            _hidWatcher = hidWatcher;
            _keyboardReaderService = keyboardReaderService;
            MessageBus.Subscribe(this);
            Events = new ObservableKeyedCollection<string, KeyPressedEvent>(e => e.KeyName);
            NotUsedKeys = new ObservableKeyedCollection<string, KeyPressedEvent>(e => e.KeyName);
        }

        public int Rows => 14;
        public int Cols => 6;
        public ObservableKeyedCollection<string, KeyPressedEvent> Events { get => _events; set => Set(ref _events, value); }
        public ObservableKeyedCollection<string, KeyPressedEvent> NotUsedKeys { get => _notUsedKeys; set => Set(ref _notUsedKeys, value); }
        public string State { get => _state; set => Set(ref _state, value); }

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
            State = "Shown";
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => _hidWatcher.Watch(_cancellationTokenSource.Token));
            Task.Run(() =>
            {
                try
                {
                    SetEvents();
                    State = "Loaded";
                }
                catch (Exception e)
                {
                    State = e.ToString();
                }
            });
        }

        private void SetEvents()
        {
            var layouts = _keyboardReaderService.LoadKeyboardLayouts().ToList();
            var manuform = layouts.Single(x => x.KeyboardName == "Dactyl Manuform 6x6");
            var layout = manuform.Layouts.Layout[0].Layout.ToDictionary(x => x.Label);
            var dispatcher = Application.Current.Dispatcher;
            for(var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
            {
                var keyName = CreateKeyName(r, c);
                var keyPressedEvent = new KeyPressedEvent(keyName);
                if (layout.ContainsKey(keyName))
                {
                    keyPressedEvent.X = layout[keyName].X * SizeOffset;
                    keyPressedEvent.Y = layout[keyName].Y * SizeOffset;
                    keyPressedEvent.IsUsed = true;
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Events.Add(keyPressedEvent)));
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => NotUsedKeys.Add(keyPressedEvent)));
                }
            }
            RaisePropertyChanged(nameof(Events));
            RaisePropertyChanged(nameof(NotUsedKeys));
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
