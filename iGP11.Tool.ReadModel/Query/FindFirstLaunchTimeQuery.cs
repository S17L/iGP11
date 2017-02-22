using System;
using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindFirstLaunchTimeQuery : IFindFirstLaunchTimeQuery
    {
        private readonly InMemoryDatabase _database;

        public FindFirstLaunchTimeQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<DateTime?> IsFirstRunAsync()
        {
            return Task.FromResult(_database.UsageStatistics.FirstLaunchTime);
        }
    }
}