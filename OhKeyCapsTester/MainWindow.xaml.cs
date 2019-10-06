using System;
using System.ComponentModel;
using System.Windows;
using CommonServiceLocator;
using OhKeyCapsTester.Contracts.Messages;
using OhKeyCapsTester.MessageBusService;

namespace OhKeyCapsTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IMessageBus>().Publish<WindowLoaded>();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IMessageBus>().Publish<WindowClosing>();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            IocContainer.CleanUp();
        }
    }
}
