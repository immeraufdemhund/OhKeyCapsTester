using System.Collections.Generic;
using OhKeyCapsTester.ViewModels;
using OhKeyCapsTester.ViewModels.Models;

namespace OhKeyCapsTester.Contracts
{
    public interface IMainWindow
    {
        IKeyboardLayoutControl KeyboardLayoutControl { get; }
        string State { get; set; }
        ICollection<string> KeyboardLayouts { get; }
        string SelectedKeyboardLayout { get; set; }
    }

    public interface IKeyboardLayoutControl
    {
        ObservableKeyedCollection<string, KeyPressedEvent> UsedKeys { get; }
        ObservableKeyedCollection<string, KeyPressedEvent> NotUsedKeys { get; }
        int Rows { get; }
        int Cols { get; }

        void SetKeyboardLayout(KeyboardLayout keyboardLayout);
    }
}
