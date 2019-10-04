using System;

namespace OhKeyCapsTester.MessageBusService
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HandleOnUIThreadAttribute : Attribute
    {
    }
}
