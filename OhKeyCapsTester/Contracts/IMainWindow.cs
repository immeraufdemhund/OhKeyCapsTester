using System.Collections.Generic;
using OhKeyCapsTester.ViewModels;

namespace OhKeyCapsTester.Contracts
{
    public interface IMainWindow
    {
        string State { get; set; }
        int Rows { get; }
        int Cols { get; }
        IDictionary<string, KeyPressedEvent> Events { get; }
    }
}
