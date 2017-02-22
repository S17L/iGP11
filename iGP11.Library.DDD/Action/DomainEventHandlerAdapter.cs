using System.Threading.Tasks;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.DDD.Action
{
    public class DomainEventHandlerAdapter<TEvent> : IListenerHandler
    {
        private readonly IDomainEventHandler<TEvent> _handler;
        private readonly IHubClientFactory _hubClientFactory;
        private readonly IEventSerializer<Event<TEvent>> _serializer;

        public DomainEventHandlerAdapter(
            IDomainEventHandler<TEvent> handler,
            IHubClientFactory hubClientFactory,
            IEventSerializer<Event<TEvent>> serializer)
        {
            _handler = handler;
            _hubClientFactory = hubClientFactory;
            _serializer = serializer;
        }

        public TypeId TypeId => _serializer.TypeId;

        public async Task HandleAsync(Event @event)
        {
            var command = _serializer.Deserialize(@event);
            await _handler.HandleAsync(
                new DomainEventContext(command.CreateEventContext(_hubClientFactory)),
                command.Data);
        }
    }
}