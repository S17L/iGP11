using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class ApplicationSettingsUpdatedEventHandler : IDomainEventHandler<ApplicationSettingsUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public ApplicationSettingsUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, ApplicationSettingsUpdatedEvent @event)
        {
            _database.ConstantSettings.ApplicationCommunicationPort = @event.ApplicationCommunicationPort;
            _database.ConstantSettings.ProxyCommunicationPort = @event.ProxyCommunicationPort;

            await context.EmitAsync(new ActionSucceededNotification());
        }
    }
}