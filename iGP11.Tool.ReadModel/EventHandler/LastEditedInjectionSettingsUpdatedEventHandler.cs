using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class LastEditedInjectionSettingsUpdatedEventHandler : IDomainEventHandler<LastEditedInjectionSettingsUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public LastEditedInjectionSettingsUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, LastEditedInjectionSettingsUpdatedEvent @event)
        {
            _database.LastEditedInjectionSettingsId = @event.Id;
            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}