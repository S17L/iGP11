using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class TextureManagementSettingsUpdatedEventHandler : IDomainEventHandler<TextureManagementSettingsUpdatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public TextureManagementSettingsUpdatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, TextureManagementSettingsUpdatedEvent @event)
        {
            _database.TextureManagementSettings = @event.Settings;
            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}