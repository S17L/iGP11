using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class InjectionSettingsUpdatedEventHandler : IDomainEventHandler<InjectionSettingsUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public InjectionSettingsUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, InjectionSettingsUpdatedEvent @event)
        {
            _database.InjectionSettings.Remove(settings => settings.Id == @event.InjectionSettings.Id);
            _database.InjectionSettings.Add(@event.InjectionSettings);

            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}