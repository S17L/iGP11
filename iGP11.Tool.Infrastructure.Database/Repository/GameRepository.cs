using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Infrastructure.Database.Model;

namespace iGP11.Tool.Infrastructure.Database.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly FileDatabaseContext _context;

        public GameRepository(FileDatabaseContext context)
        {
            _context = context;
        }

        public async Task ChangeGameProfileAsync(AggregateId profileId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                if (_context.Games.All(game => game.Profiles.All(profile => profile.Id != profileId)))
                {
                    throw new AggregateRootNotFoundException($"game profile with id: {profileId} could not be found");
                }

                _context.LastEditedProfileId = profileId;
                _context.Commit();
            }
        }

        public async Task<IEnumerable<Game>> LoadAllAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _context.Games.Clone();
            }
        }

        public async Task<Game> LoadAsync(AggregateId gameId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var model = FindById(gameId);
                if (model == null)
                {
                    throw new AggregateRootNotFoundException($"game with id: {gameId} could not be found");
                }

                return model.Clone();
            }
        }

        public async Task<Game> LoadByGameProfileId(AggregateId profileId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return FindByProfileId(profileId);
            }
        }

        public async Task<AggregateId> LoadGameProfileIdAsync()
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                return _context.LastEditedProfileId;
            }
        }

        public async Task RemoveGameAsync(AggregateId gameId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var game = FindById(gameId);
                if (game == null)
                {
                    throw new AggregateRootNotFoundException($"game with id: {gameId} could not be found");
                }

                _context.Games.Remove(game);
                if (game.Profiles.Any(entity => entity.Id == _context.LastEditedProfileId))
                {
                    _context.LastEditedProfileId = FindFirstProfileId();
                }

                _context.Commit();
            }
        }

        public async Task SaveAsync(Game game)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _context.Games.Remove(entity => entity.Id == game.Id);
                _context.Games.Add(game.Clone());
                _context.LastEditedProfileId = game.ProfileId;
                _context.Commit();
            }
        }

        private Game FindById(AggregateId gameId)
        {
            return _context.Games.SingleOrDefault(entity => entity.Id == gameId);
        }

        private Game FindByProfileId(AggregateId profileId)
        {
            return _context.Games.SingleOrDefault(game => game.Profiles.Any(profile => profile.Id == profileId));
        }

        private AggregateId FindFirstProfileId()
        {
            return _context.Games.FirstOrDefault()?.Profiles.FirstOrDefault()?.Id;
        }
    }
}