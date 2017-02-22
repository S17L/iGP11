using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public class PublishSettings<TEvent>
    {
        public EndpointId? DestinationEndpointId { get; set; }

        public IEventSerializer<TEvent> Serializer { get; set; }
    }
}