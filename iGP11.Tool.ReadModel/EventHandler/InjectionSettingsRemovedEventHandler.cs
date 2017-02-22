using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class InjectionSettingsRemovedEventHandler : IDomainEventHandler<InjectionSettingsRemovedEvent>
    {
        private readonly InMemoryDatabase _database;

        public InjectionSettingsRemovedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, InjectionSettingsRemovedEvent @event)
        {
            _database.InjectionSettings.Remove(settings => settings.Id == @event.Id);
            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}