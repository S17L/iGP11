using System.Collections.Concurrent;
using System.Collections.Generic;

namespace iGP11.Library.Scheduler
{
    public class SchedulerTaskCollection : ISchedulerSubscriber
    {
        private readonly ConcurrentQueue<IScheduledTask> _tasks;

        public SchedulerTaskCollection(IEnumerable<IScheduledTask> tasks)
        {
            _tasks = new ConcurrentQueue<IScheduledTask>(tasks);
        }

        public bool TryScheduleTask(out IScheduledTask task)
        {
            return _tasks.TryDequeue(out task);
        }
    }
}