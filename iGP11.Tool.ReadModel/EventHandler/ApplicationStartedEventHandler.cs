using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ApplicationStartedEventHandler : IDomainEventHandler<ApplicationStartedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ApplicationStartedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, ApplicationStartedEvent @event)
        {
            _database.InjectionStatuses[@event.ApplicationFilePath] = @event.Status;
            await context.EmitAsync(new Shared.Notification.ApplicationStartedEvent(@event.Status));
        }
    }
}