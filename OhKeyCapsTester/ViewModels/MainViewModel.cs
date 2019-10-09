using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using OhKeyCapsTester.ViewModels.Models;

namespace OhKeyCapsTester.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainWindow, IHandle<WindowLoaded>, IHandle<WindowClosing>, IHandle<DeviceFound>
    {
        private string _state;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly IHidWatcher _hidWatcher;
        private readonly IKeyboardReaderService _keyboardReaderService;
        private Dictionary<string, KeyboardLayout> _layouts;
        private string _selectedKeyboardLayout;

        public MainViewModel(IMessageBus messageBus, IHidWatcher hidWatcher, IKeyboardReaderService keyboardReaderService, IKeyboardLayoutControl keyboardLayoutControl) : base(messageBus)
        {
            _hidWatcher = hidWatcher;
            _keyboardReaderService = keyboardReaderService;
            KeyboardLayoutControl = keyboardLayoutControl;
            KeyboardLayouts = new ObservableCollection<string>();
            MessageBus.Subscribe(this);
        }

        public IKeyboardLayoutControl KeyboardLayoutControl { get; }

        public string State { get => _state; set => Set(ref _state, value); }
        public ICollection<string> KeyboardLayouts { get; }

        public string SelectedKeyboardLayout
        {
            get => _selectedKeyboardLayout;
            set
            {
                Set(ref _selectedKeyboardLayout, value);
                KeyboardLayoutControl.SetKeyboardLayout(_layouts[value]);
            }
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
                    LoadKeyboardLayouts();
                    State = "Loaded";
                }
                catch (Exception e)
                {
                    State = e.ToString();
                }
            });
        }

        private void LoadKeyboardLayouts()
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher == null) return;
            _layouts = _keyboardReaderService.LoadKeyboardLayouts().ToDictionary(x => x.KeyboardName);
            dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                KeyboardLayouts.Clear();
                _layouts.Keys.ForEach(KeyboardLayouts.Add);
            }));
        }

        public void Handle(WindowClosing message)
        {
            _cancellationTokenSource.Cancel();
        }

        public void Handle(DeviceFound message)
        {
            State = "Listening for device";
        }
    }
}
