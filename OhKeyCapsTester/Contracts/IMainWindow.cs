using System.Collections.Generic;
using OhKeyCapsTester.ViewModels;

namespace OhKeyCapsTester.Contracts
{
    public interface IMainWindow
    {
        string State { get; set; }
        int Rows { get; }
        int Cols { get; }
        ObservableKeyedCollection<string, KeyPressedEvent> Events { get; }
        ObservableKeyedCollection<string, KeyPressedEvent> NotUsedKeys { get; }
    }
}
