using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class InjectionSettingsAddedEventHandler : IDomainEventHandler<InjectionSettingsAddedEvent>
    {
        private readonly InMemoryDatabase _database;

        public InjectionSettingsAddedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, InjectionSettingsAddedEvent @event)
        {
            _database.InjectionSettings.Add(@event.InjectionSettings);
            await context.EmitAsync(new Shared.Notification.InjectionSettingsAddedEvent(@event.InjectionSettings.Id));
        }
    }
}