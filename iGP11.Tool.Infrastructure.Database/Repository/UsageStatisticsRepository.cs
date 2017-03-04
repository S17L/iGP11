using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.UsageStatistics;
using iGP11.Tool.Infrastructure.Database.Model;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class UsageStatisticsRepository : IUsageStatisticsRepository
    {
        private readonly FileDatabaseContext _context;

        public UsageStatisticsRepository(FileDatabaseContext context)
        {
            _context = context;
        }

        public async Task<UsageStatistics> LoadAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = _context.UsageStatistics;
                if (model == null)
                {
                    throw new AggregateRootNotFoundException("usage statistics not found");
                }

                return model.Clone();
            }
        }

        public async Task SaveAsync(UsageStatistics usageStatistics)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.UsageStatistics = usageStatistics.Clone();
                _context.Commit();
            }
        }
    }
}