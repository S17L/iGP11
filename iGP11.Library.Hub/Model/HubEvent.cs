using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Model
{
    [DataContract]
    public class HubEvent : IEvent
    {
        public HubEvent(Event @event, IEnumerable<EndpointId> endpointIds)
        {
            Event = @event;
            EndpointIds = endpointIds;
        }

        [DataMember(Name = "events", IsRequired = true)]
        public IEnumerable<EndpointId> EndpointIds { get; private set; }

        [DataMember(Name = "event", IsRequired = true)]
        public Event Event { get; private set; }

        [DataMember(Name = "eventId", IsRequired = true)]
        public EventId EventId { get; private set; } = EventId.Generate();

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