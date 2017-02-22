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

        public Task<TextureManagementSettings> FindAsync()
        {
            return Task.FromResult(_database.TextureManagementSettings.Clone());
        }
    }
}