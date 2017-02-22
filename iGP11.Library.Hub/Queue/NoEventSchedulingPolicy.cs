using System;

namespace iGP11.Library.Hub.Queue
{
    public class NoEventSchedulingPolicy : IEventSchedulingPolicy
    {
        public DateTime? GetNextScheduledExecutionTime(int retryCount, DateTime? lastExecutionTime)
        {
            return null;
        }
    }
}