using System;
using System.Reflection;
using System.Windows.Threading;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using OhKeyCapsTester.Contracts;
using OhKeyCapsTester.MessageBusService;
using OhKeyCapsTester.ViewModels;

namespace OhKeyCapsTester
{
    public static class IocContainer
    {
        public static IContainer BaseContainer { get; private set; }
        internal static ServiceLocatorProvider ServiceLocatorProvider => () => new AutofacServiceLocator(BaseContainer);

        public static void Build()
        {
            if (BaseContainer != null) return;
            var builder = new ContainerBuilder();

            builder.RegisterInstance<IMessageBus>(new MessageBus(HandleTheadOnUiThread, Dispatcher.CurrentDispatcher.CheckAccess));
            var assemblies = new[] {Assembly.GetExecutingAssembly()};

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IViewModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof(IService).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            BaseContainer = builder.Build();
        }

        private static void HandleTheadOnUiThread(Action action)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, action);
        }

        internal static void CleanUp()
        {
            using (BaseContainer) { }
        }
    }
}
