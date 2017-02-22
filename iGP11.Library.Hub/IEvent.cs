using System;

using iGP11.Library.Hub.Model;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub
{
    public interface IEvent
    {
        EventId EventId { get; }

        DateTime? LastExecutionTime { get; set; }

        int RetryCount { get; set; }

        DateTime? ScheduledExecutionTime { get; set; }

        DeliveryStatus Status { get; set; }
    }
}