using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public sealed class HubClientFactory : IHubClientFactory
    {
        private readonly IEventSerializerFactory _defaultSerializerFactory;
        private readonly IHubClientTransport _hubClientTransport;

        public HubClientFactory(IHubClientTransport transport, IEventSerializerFactory defaultSerializerFactory)
        {
            _hubClientTransport = transport;
            _defaultSerializerFactory = defaultSerializerFactory;
        }

        public IHubClient Create(EndpointId id)
        {
            var hubClient = new HubClient(id, _hubClientTransport, _defaultSerializerFactory);
            hubClient.Connect();

            return hubClient;
        }
    }
}