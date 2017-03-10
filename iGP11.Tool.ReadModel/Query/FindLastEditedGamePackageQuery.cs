using System.Threading.Tasks;

using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Model;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindLastEditedGamePackageQuery : IFindLastEditedGamePackageQuery
    {
        private readonly InMemoryDatabase _database;
        private readonly IFindGamePackageByIdQuery _query;

        public FindLastEditedGamePackageQuery(InMemoryDatabase database, IFindGamePackageByIdQuery query)
        {
            _database = database;
            _query = query;
        }

        public Task<GamePackage> FindAsync()
        {
            var id = _database.LastEditedGameProfileId;
            return id.HasValue
                       ? _query.FindByGameProfileIdAsync(id.Value)
                       : Task.FromResult<GamePackage>(null);
        }
    }
}