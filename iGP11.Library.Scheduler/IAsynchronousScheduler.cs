using System.Threading.Tasks;

namespace iGP11.Library.Scheduler
{
    public interface IAsynchronousScheduler : IScheduler
    {
        Task StartAsync();

        void Subscribe(ISchedulerSubscriber subscriber);

        void Unsubscribe(ISchedulerSubscriber subscriber);
    }
}