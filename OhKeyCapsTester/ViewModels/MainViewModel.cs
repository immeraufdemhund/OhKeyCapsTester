using OhKeyCapsTester.Contracts;
using OhKeyCapsTester.Contracts.Messages;
using OhKeyCapsTester.MessageBusService;

namespace OhKeyCapsTester.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainWindow, IHandle<WindowLoaded>
    {
        private string _state;
        public string State { get => _state; set => Set(ref _state, value); }

        public MainViewModel(IMessageBus messageBus) : base(messageBus)
        {
            MessageBus.Subscribe(this);
        }

        public void Handle(WindowLoaded message)
        {
            State = "Loaded";
        }
    }
}
