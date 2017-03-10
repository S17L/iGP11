using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model.TextureManagementSettings;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindTextureManagementSettingsQuery : IFindTextureManagementSettingsQuery
    {
        private readonly InMemoryDatabase _database;

        public FindTextureManagementSettingsQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task<TextureManagementSettings> FindAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _database.TextureManagementSettings.Clone();
            }
        }
    }
}