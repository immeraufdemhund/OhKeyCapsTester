using System;
using System.Windows.Input;

namespace OhKeyCapsTester.ViewModels
{
    public class CommandHandler : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;
        private readonly bool _checkIfExecuteChanged;

        public CommandHandler(Action action, Func<bool> canExecute, bool checkIfExecuteChanged = false)
            : this(x => { action(); }, y => { return canExecute(); }, checkIfExecuteChanged)
        {
        }

        public CommandHandler(Action<object> action, Func<object, bool> canExecute, bool checkIfExecuteChanged = false)
        {
            _action = action;
            _canExecute = canExecute;
            _checkIfExecuteChanged = checkIfExecuteChanged;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_checkIfExecuteChanged)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_checkIfExecuteChanged)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
