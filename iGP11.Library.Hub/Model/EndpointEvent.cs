using System;
using System.Runtime.Serialization;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Model
{
    [DataContract]
    public class EndpointEvent : IEvent
    {
        public EndpointEvent(EventId hubEventId, EndpointId endpointId, Event @event)
        {
            HubEventId = hubEventId;
            EndpointId = endpointId;
            Event = @event;
        }

        [DataMember(Name = "endpointId", IsRequired = true)]
        public EndpointId EndpointId { get; private set; }

        [DataMember(Name = "event", IsRequired = true)]
        public Event Event { get; private set; }

        [DataMember(Name = "eventId", IsRequired = true)]
        public EventId EventId { get; private set; } = EventId.Generate();

        [DataMember(Name = "hubEventId", IsRequired = true)]
        public EventId HubEventId { get; private set; }

        [DataMember(Name = "lastExecutionTime", IsRequired = true)]
        public DateTime? LastExecutionTime { get; set; }

        [DataMember(Name = "retryCount", IsRequired = true)]
        public int RetryCount { get; set; }

        [DataMember(Name = "scheduledExecutionTime", IsRequired = true)]
        public DateTime? ScheduledExecutionTime { get; set; }

        [DataMember(Name = "status", IsRequired = true)]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    }
}