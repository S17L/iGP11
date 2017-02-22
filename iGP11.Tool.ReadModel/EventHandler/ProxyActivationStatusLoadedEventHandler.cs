using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ProxyActivationStatusLoadedEventHandler : IDomainEventHandler<ProxyActivationStatusLoadedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ProxyActivationStatusLoadedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, ProxyActivationStatusLoadedEvent @event)
        {
            _database.ProxyActivationStatuses[@event.ApplicationFilePath] = @event.ActivationStatus;
            await context.EmitAsync(new Shared.Notification.ProxyActivationStatusLoadedEvent(@event.ActivationStatus));
        }
    }
}