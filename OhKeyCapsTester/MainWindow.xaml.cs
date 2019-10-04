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
    }
}
