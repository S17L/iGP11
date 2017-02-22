using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client.Action
{
    public class FeedbackEventContext
    {
        private readonly IHubClientFactory _hubClientFactory;
        private readonly EndpointId _id;

        public FeedbackEventContext(
            EndpointId id,
            IHubClientFactory hubClientFactory)
        {
            _id = id;
            _hubClientFactory = hubClientFactory;
        }

        public async Task EmitAsync<TEvent>(TEvent @event, IEventSerializer<TEvent> serializer = null)
        {
            using (var hubClient = _hubClientFactory.Create(EndpointId.Generate()))
            {
                var settings = new PublishSettings<TEvent>
                {
                    DestinationEndpointId = _id,
                    Serializer = serializer
                };

                await hubClient.PublishAsync(@event, settings);
            }
        }

        public async Task PublishAsync<TEvent>(TEvent @event, PublishSettings<Event<TEvent>> settings = null)
        {
            using (var hubClient = _hubClientFactory.Create(EndpointId.Generate()))
            {
                await hubClient.PublishAsync(new Event<TEvent>(@event, _id), settings);
            }
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}