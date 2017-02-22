using System;

namespace iGP11.Library.Scheduler
{
    public sealed class DisabledSchedulerScope : IDisposable
    {
        private readonly bool _isRunning;
        private readonly IScheduler _scheduler;

        public DisabledSchedulerScope(IScheduler scheduler)
        {
            _scheduler = scheduler;
            _isRunning = scheduler?.IsRunning ?? false;
            _scheduler?.Stop();
        }

        public void Dispose()
        {
            if (_isRunning)
            {
                _scheduler?.Start();
            }
        }
    }
}