using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.ReadModel.Api.Exception;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class GameProfileAddedEventHandler : IDomainEventHandler<GameProfileAddedEvent>
    {
        private readonly InMemoryDatabase _database;

        public GameProfileAddedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameProfileAddedEvent @event)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var game = FindGameById(@event.GameId);
                if (game == null)
                {
                    throw new EntityNotFoundException($"game with id: {@event.GameId} could not be found");
                }

                game.ProfileId = @event.GameProfileId;
                game.Profiles.Add(@event.GameProfile);
            }

            await context.EmitAsync(new GameProfileAddedNotification(@event.GameProfile.Id));
        }

        private Game FindGameById(Guid id)
        {
            return _database.Games.FirstOrDefault(game => game.Id == id);
        }
    }
}