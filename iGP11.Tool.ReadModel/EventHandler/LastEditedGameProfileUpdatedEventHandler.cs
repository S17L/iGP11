using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class LastEditedGameProfileUpdatedEventHandler : IDomainEventHandler<LastEditedGameProfileUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public LastEditedGameProfileUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, LastEditedGameProfileUpdatedEvent @event)
        {
            _database.LastEditedGameProfileId = @event.Id;
            await context.EmitAsync(new ActionSucceededNotification());
        }
    }
}