using System.Runtime.Serialization;

namespace iGP11.Library.Hub.Shared
{
    [DataContract]
    public class HubClientEvent
    {
        public HubClientEvent(Event @event, EndpointId? recipientId)
        {
            Event = @event;
            RecipientId = recipientId;
        }

        [DataMember(Name = "event")]
        public Event Event { get; private set; }

        [DataMember(Name = "recipientId")]
        public EndpointId? RecipientId { get; set; }
    }
}