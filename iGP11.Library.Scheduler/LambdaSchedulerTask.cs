using System;
using System.Threading.Tasks;

namespace iGP11.Library.Scheduler
{
    public class LambdaSchedulerTask : IScheduledTask
    {
        private readonly Func<Task> _lambda;

        public LambdaSchedulerTask(Func<Task> lambda)
        {
            _lambda = lambda;
        }

        public Task ExecuteAsync()
        {
            return _lambda();
        }
    }
}