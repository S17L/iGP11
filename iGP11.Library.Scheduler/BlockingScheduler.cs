using System;
using System.Threading;

using Timer = System.Timers.Timer;

namespace iGP11.Library.Scheduler
{
    public sealed class BlockingScheduler : IScheduler
    {
        private readonly Action _action;
        private readonly object _lock = new object();
        private readonly Timer _timer;

        private bool? _isRunning = false;

        public BlockingScheduler(Action action, int interval)
        {
            _action = action;
            _timer = new Timer
            {
                Interval = interval
            };

            _timer.Elapsed += (sender, args) => Execute();
        }

        public bool IsRunning => _isRunning.GetValueOrDefault();

        public void Dispose()
        {
            if (_isRunning == null)
            {
                return;
            }

            try
            {
                _timer.Dispose();
            }
            finally
            {
                _isRunning = null;
            }
        }

        public void Start()
        {
            if (!_isRunning.HasValue || _isRunning.Value)
            {
                return;
            }

            _timer.Start();
            _isRunning = true;
        }

        public void Stop()
        {
            if (!_isRunning.HasValue || !_isRunning.Value)
            {
                return;
            }

            _timer.Stop();
            _isRunning = false;
        }

        private void Execute()
        {
            if (!Monitor.TryEnter(_lock))
            {
                return;
            }

            try
            {
                _action();
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}