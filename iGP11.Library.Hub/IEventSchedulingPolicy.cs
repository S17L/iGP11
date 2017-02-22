using System;

namespace iGP11.Library.Hub
{
    public interface IEventSchedulingPolicy
    {
        DateTime? GetNextScheduledExecutionTime(int retryCount, DateTime? lastExecutionTime);
    }
}