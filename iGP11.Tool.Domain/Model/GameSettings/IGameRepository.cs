using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    public interface IGameRepository : IRepository<Game, AggregateId>
    {
        Task ChangeGameProfileAsync(AggregateId profileId);

        Task<IEnumerable<Game>> LoadAllAsync();

        Task<Game> LoadByGameProfileId(AggregateId profileId);

        Task<AggregateId> LoadGameProfileIdAsync();

        Task RemoveGameAsync(AggregateId gameId);
    }
}