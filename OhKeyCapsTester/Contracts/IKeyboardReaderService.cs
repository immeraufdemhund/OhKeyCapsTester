using System.Collections.Generic;

namespace OhKeyCapsTester.Contracts
{
    public interface IKeyboardReaderService : IService
    {
        IEnumerable<KeyboardLayout> LoadKeyboardLayouts();
    }

    public struct KeyboardLayout
    {
        public string KeyboardName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public KeyLayoutCollection Layouts { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }

    public struct KeyLayoutCollection
    {
        public KeyLayout[] Layout { get; set; }
        public int Count => Layout?.Length ?? 0;

        public KeyLayout this[int i] => Layout[i];
    }

    public struct KeyLayout
    {
        public string Name { get; set; }
        public KeyLayoutCoordinate[] Layout { get; set; }
    }

    public struct KeyLayoutCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int H { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return $"[{nameof(Label)}: {Label}]{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(H)}: {H}";
        }
    }
}
