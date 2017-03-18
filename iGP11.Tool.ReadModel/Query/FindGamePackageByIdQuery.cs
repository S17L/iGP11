using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.ReadModel.Api.Exception;
using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.ReadModel.Query
{
    public class FindGamePackageByIdQuery : IFindGamePackageByIdQuery
    {
        private readonly InMemoryDatabase _database;

        public FindGamePackageByIdQuery(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task<GamePackage> FindByGameIdAsync(Guid gameId)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var game = FindGameById(gameId);
                if (game == null)
                {
                    throw new EntityNotFoundException($"game with id: {gameId} could not be found");
                }

                var gameProfile = FindGameProfileById(game, game.ProfileId);
                if (gameProfile == null)
                {
                    throw new EntityNotFoundException($"game profile with id: {game.ProfileId} could not be found");
                }

                return new GamePackage(game, gameProfile).Clone();
            }
        }

        public Task<GamePackage> FindByGameProfileIdAsync(Guid gameProfileId)
        {
            var game = FindGameByProfileId(gameProfileId);
            if (game == null)
            {
                throw new EntityNotFoundException($"game with profile id: {gameProfileId} could not be found");
            }

            var gameProfile = FindGameProfileById(game, gameProfileId);
            if (gameProfile == null)
            {
                throw new EntityNotFoundException($"game profile with id: {gameProfileId} could not be found");
            }

            return Task.FromResult(new GamePackage(game, gameProfile).Clone());
        }

        private static GameProfile FindGameProfileById(Game game, Guid gameProfileId)
        {
            return game.Profiles.SingleOrDefault(gameProfile => gameProfile.Id == gameProfileId);
        }

        private Game FindGameById(Guid gameId)
        {
            return _database.Games.SingleOrDefault(game => game.Id == gameId);
        }

        private Game FindGameByProfileId(Guid gameProfileId)
        {
            return _database.Games.SingleOrDefault(game => game.Profiles.Any(gameProfile => gameProfile.Id == gameProfileId));
        }
    }
}