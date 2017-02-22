using System.Threading.Tasks;

namespace iGP11.Library.Scheduler
{
    public interface IScheduledTask
    {
        Task ExecuteAsync();
    }
}