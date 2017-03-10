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
    public class GameUpdatedEventHandler : IDomainEventHandler<GameUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public GameUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameUpdatedEvent @event)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                var game = FindGameById(@event.Id);
                if (game == null)
                {
                    throw new EntityNotFoundException($"game with id: {@event.Id} could not be found");
                }

                game.Name = @event.Name;
                game.FilePath = @event.FilePath;
            }

            await context.EmitAsync(new ActionSucceededNotification());
        }

        private Game FindGameById(Guid id)
        {
            return _database.Games.SingleOrDefault(game => game.Id == id);
        }
    }
}