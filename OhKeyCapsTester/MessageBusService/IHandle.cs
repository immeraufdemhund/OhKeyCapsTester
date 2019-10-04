namespace OhKeyCapsTester.MessageBusService
{
    public interface IHandle{}
    public interface IHandle<in T> : IHandle
    {
        /// <summary>
        ///     Handles the given message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Handle(T message);
    }
}
