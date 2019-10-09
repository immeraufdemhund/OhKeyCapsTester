namespace OhKeyCapsTester.ViewModels.Models
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
}