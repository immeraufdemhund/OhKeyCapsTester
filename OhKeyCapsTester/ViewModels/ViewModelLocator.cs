using CommonServiceLocator;
using OhKeyCapsTester.Contracts;

namespace OhKeyCapsTester.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                IocContainer.Build();
            }

            var _ = IocContainer.BaseContainer;
            ServiceLocator.SetLocatorProvider(IocContainer.ServiceLocatorProvider);
        }

        public IMainWindow Main => ServiceLocator.Current.GetInstance<IMainWindow>();

        public static void CleanUp()
        {
            IocContainer.CleanUp();
        }
    }
}
