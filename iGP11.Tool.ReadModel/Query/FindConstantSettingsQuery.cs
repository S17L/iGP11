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

        public async Task<ConstantSettings> FindAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _database.ConstantSettings.Clone();
            }
        }
    }
}