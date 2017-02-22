using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindConstantSettingsQuery : IFindConstantSettingsQuery
    {
        private readonly InMemoryDatabase _database;

        public FindConstantSettingsQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public Task<ConstantSettings> FindAsync()
        {
            return Task.FromResult(_database.ConstantSettings.Clone());
        }
    }
}