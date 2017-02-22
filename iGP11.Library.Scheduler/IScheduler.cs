using System;

namespace iGP11.Library.Scheduler
{
    public interface IScheduler : IDisposable
    {
        bool IsRunning { get; }

        void Start();

        void Stop();
    }
}