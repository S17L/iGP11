using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindPluginStateQuery : IFindPluginStateQuery
    {
        private readonly InMemoryDatabase _database;

        public FindPluginStateQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<ProxySettings> FindPluginStateAsync()
        {
            return Task.FromResult(_database.PluginState);
        }
    }
}