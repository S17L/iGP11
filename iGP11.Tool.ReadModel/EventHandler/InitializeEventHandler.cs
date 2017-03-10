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
            using (await IsolatedDatabaseAccess.Open())
            {
                _database.ConstantSettings.ApplicationCommunicationPort = @event.ApplicationSettings.ApplicationCommunicationPort;
                _database.ConstantSettings.ProxyCommunicationPort = @event.ApplicationSettings.ProxyCommunicationPort;
                _database.Games.Clear();
                _database.Games.AddRange(@event.Games);
                _database.LastEditedGameProfileId = @event.LastEditedGameProfileId;
                _database.TextureManagementSettings = @event.TextureManagementSettings;
                _database.UsageStatistics = @event.UsageStatistics;
            }

            await context.EmitAsync(new ActionSucceededNotification());
        }
    }
}