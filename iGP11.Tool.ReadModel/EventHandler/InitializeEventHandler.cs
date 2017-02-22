using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class InitializeEventHandler : IDomainEventHandler<InitializeEvent>
    {
        private readonly InMemoryDatabase _database;

        public InitializeEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, InitializeEvent @event)
        {
            _database.ConstantSettings.ApplicationCommunicationPort = @event.ApplicationSettings.ApplicationCommunicationPort;
            _database.ConstantSettings.ProxyCommunicationPort = @event.ApplicationSettings.ProxyCommunicationPort;
            _database.InjectionSettings.Clear();
            _database.InjectionSettings.AddRange(@event.InjectionSettings);
            _database.LastEditedInjectionSettingsId = @event.LastEditedInjectionSettingsId;
            _database.TextureManagementSettings = @event.TextureManagementSettings;
            _database.UsageStatistics = @event.UsageStatistics;

            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}