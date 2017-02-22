using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ReadModel.EventHandler
{
    public class FirstLaunchIndicatedEventHandler : IDomainEventHandler<FirstLaunchIndicatedEvent>
    {
        private readonly InMemoryDatabase _database;

        public FirstLaunchIndicatedEventHandler(InMemoryDatabase database)
        {
            _database = database;
        }

        public async Task HandleAsync(DomainEventContext context, FirstLaunchIndicatedEvent @event)
        {
            _database.UsageStatistics.FirstLaunchTime = @event.Time;
            await context.EmitAsync(new ActionSucceededEvent());
        }
    }
}