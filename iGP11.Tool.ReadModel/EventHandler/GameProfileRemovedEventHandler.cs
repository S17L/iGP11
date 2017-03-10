using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.ReadModel.Api.Exception;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class GameProfileRemovedEventHandler : IDomainEventHandler<GameProfileRemovedEvent>
    {
        private readonly InMemoryDatabase _database;

        public GameProfileRemovedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameProfileRemovedEvent @event)
        {
            var game = FindGameByProfileId(@event.RemovedGameProfileId);
            if (game == null)
            {
                throw new EntityNotFoundException($"game with game profile id: {@event.RemovedGameProfileId} could not be found");
            }

            _database.LastEditedGameProfileId = @event.LastEditedGameProfileId;
            game.ProfileId = @event.LastEditedGameProfileId;
            game.Profiles.Remove(gameProfile => gameProfile.Id == @event.RemovedGameProfileId);

            await context.EmitAsync(new ActionSucceededNotification());
        }

        private Game FindGameByProfileId(Guid gameProfileId)
        {
            return _database.Games.FirstOrDefault(game => game.Profiles.Any(gameProfile => gameProfile.Id == gameProfileId));
        }
    }
}