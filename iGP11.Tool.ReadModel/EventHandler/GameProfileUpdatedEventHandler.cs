using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class GameProfileUpdatedEventHandler : IDomainEventHandler<GameProfileUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public GameProfileUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameProfileUpdatedEvent @event)
        {
            var game = FindGameByProfileId(@event.GameProfile.Id);
            game?.Profiles.Remove(gameProfile => gameProfile.Id == @event.GameProfile.Id);
            game?.Profiles.Add(@event.GameProfile);

            await context.EmitAsync(new ActionSucceededNotification());
        }

        private Game FindGameByProfileId(Guid gameProfileId)
        {
            return _database.Games.FirstOrDefault(game => game.Profiles.Any(gameProfile => gameProfile.Id == gameProfileId));
        }
    }
}