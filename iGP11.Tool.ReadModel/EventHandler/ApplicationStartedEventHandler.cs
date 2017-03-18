using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ApplicationStartedEventHandler : IDomainEventHandler<GameStartedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ApplicationStartedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, GameStartedEvent @event)
        {
            using (await IsolatedDatabaseAccess.Open())
            {
                _database.GameLaunchingStatuses[@event.FilePath] = @event.Status;
            }

            await context.EmitAsync(new ApplicationStartedNotification(@event.Status));
        }
    }
}