using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.UsageStatistics
{
    public class UsageStatisticsFactory
    {
        public UsageStatistics Create()
        {
            return new UsageStatistics(AggregateId.Generate());
        }
    }
}