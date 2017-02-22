using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public sealed class Listener : IListener
    {
        private readonly IListenerHandler _handler;
        private readonly HubClient _hubClient;

        public Listener(HubClient hubClient, IListenerHandler handler)
        {
            _hubClient = hubClient;
            _handler = handler;
        }

        public TypeId TypeId => _handler.TypeId;

        public async Task DeliverAsync(Event @event)
        {
            await _handler.HandleAsync(@event);
        }

        public void Dispose()
        {
            _hubClient.Drop(this);
        }
    }
}