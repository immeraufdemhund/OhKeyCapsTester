using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using OhKeyCapsTester.Contracts;
using OhKeyCapsTester.Contracts.Messages;
using OhKeyCapsTester.MessageBusService;
using OhKeyCapsTester.ViewModels.Models;

namespace OhKeyCapsTester.ViewModels
{
    public class KeyboardLayoutViewModel : ViewModelBase, IKeyboardLayoutControl, IHandle<KeyEvent>
    {
        private readonly Lazy<int> _halfRows;

        public KeyboardLayoutViewModel(IMessageBus messageBus) : base(messageBus)
        {
            MessageBus.Subscribe(this);
            _halfRows = new Lazy<int>(() => Rows/2);
            UsedKeys = new ObservableKeyedCollection<string, KeyPressedEvent>(e => e.KeyName);
            NotUsedKeys = new ObservableKeyedCollection<string, KeyPressedEvent>(e => e.KeyName);
        }
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public ObservableKeyedCollection<string, KeyPressedEvent> UsedKeys { get; }
        public ObservableKeyedCollection<string, KeyPressedEvent> NotUsedKeys { get; }

        public void SetKeyboardLayout(KeyboardLayout keyboardLayout)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null) return;
            Rows = keyboardLayout.Rows;
            Cols = keyboardLayout.Cols;

            var layout = keyboardLayout.Layouts.Layout[0].Layout.ToDictionary(x => x.Label);
            dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UsedKeys.Clear()));
            dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => NotUsedKeys.Clear()));

            for(var r = 0; r < Rows; r++)
            for (var c = 0; c < Cols; c++)
            {
                var keyName = CreateKeyName(r, c);
                var keyPressedEvent = new KeyPressedEvent(keyName);
                if (layout.ContainsKey(keyName))
                {
                    keyPressedEvent.X = layout[keyName].X;
                    keyPressedEvent.Y = layout[keyName].Y;
                    keyPressedEvent.IsUsed = true;
                    dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => UsedKeys.Add(keyPressedEvent)));
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => NotUsedKeys.Add(keyPressedEvent)));
                }
            }
        }

        public void Handle(KeyEvent message)
        {
            var index = CreateKeyName(message.Row, message.Col);
            if (!UsedKeys.Contains(index)) return;
            UsedKeys[index].PressedCount += message.Pressed;
            UsedKeys[index].Pressed = message.Pressed;
        }

        private string CreateKeyName(int r, int c)
        {
            var r2 = _halfRows.Value;
            var isRightHand = r >= r2;
            var lr = isRightHand ? "R" : "L";
            var ar = isRightHand ? r - r2 : r;
            return $"{lr}{ar}{c}";
        }
    }
}
