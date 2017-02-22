namespace iGP11.Library.Scheduler
{
    public interface ISchedulerSubscriber
    {
        bool TryScheduleTask(out IScheduledTask task);
    }
}