using System.Threading.Tasks;

namespace iGP11.Tool.Domain.Model.UsageStatistics
{
    public interface IUsageStatisticsRepository
    {
        Task<UsageStatistics> LoadAsync();

        Task SaveAsync(UsageStatistics usageStatistics);
    }
}