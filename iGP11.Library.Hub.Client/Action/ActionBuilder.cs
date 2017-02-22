namespace iGP11.Library.Hub.Client.Action
{
    public class ActionBuilder
    {
        private readonly IEventSerializerFactory _defaultSerializerFactory;
        private readonly IHubClientFactory _hubClientFactory;

        public ActionBuilder(IHubClientFactory hubClientFactory, IEventSerializerFactory defaultSerializerFactory)
        {
            _hubClientFactory = hubClientFactory;
            _defaultSerializerFactory = defaultSerializerFactory;
        }

        public ListenerBuilder<TCommand> Dispatch<TCommand>(TCommand command, PublishSettings<Event<TCommand>> publishSettings)
        {
            return new ListenerBuilder<TCommand>(
                command,
                publishSettings,
                _hubClientFactory,
                _defaultSerializerFactory);
        }
    }
}