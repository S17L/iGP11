using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ProxySettingsLoadedEventHandler : IDomainEventHandler<ProxySettingsLoadedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ProxySettingsLoadedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, ProxySettingsLoadedEvent @event)
        {
            _database.ProxySettings = @event.ProxySettings;
            await context.EmitAsync(new Shared.Notification.ProxySettingsLoadedNotification(@event.ProxySettings));
        }
    }
}