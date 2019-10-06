using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using OhKeyCapsTester.MessageBusService;

namespace OhKeyCapsTester.ViewModels
{
    public interface IViewModel
    {
    }

    public abstract class NotifyPropertyChanged: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool Set<T>(ref T field,
            T newValue = default,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            var oldValue = field;
            field = newValue;

            RaisePropertyChanged(propertyName);

            return true;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class ViewModelBase : NotifyPropertyChanged, IViewModel
    {
        private static bool? _isInDesignMode;

        public ViewModelBase(IMessageBus messageBus)
        {
            messageBus.Subscribe(this);
            MessageBus = messageBus;
        }

        public static bool IsInDesignModeStatic
        {
            get
            {
                if (_isInDesignMode.HasValue) return _isInDesignMode.Value;
                var prop = DesignerProperties.IsInDesignModeProperty;

                _isInDesignMode
                    = (bool) DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata.DefaultValue;

                // Just to be sure
                if (!_isInDesignMode.Value
                    && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                {
                    _isInDesignMode = true;
                }

                return _isInDesignMode.Value;
            }
        }

        protected IMessageBus MessageBus { get; }
    }
}
