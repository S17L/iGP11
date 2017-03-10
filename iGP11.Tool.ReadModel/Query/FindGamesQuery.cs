using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindGamesQuery : IFindGamesQuery
    {
        private readonly InMemoryDatabase _database;

        public FindGamesQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<Game>> FindAllAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _database.Games.Clone();
            }
        }
    }
}