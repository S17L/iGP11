using System.Threading.Tasks;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.DDD.Action
{
    public class DomainCommandHandlerAdapter<TCommand> : IListenerHandler
    {
        private readonly IDomainCommandHandler<TCommand> _handler;
        private readonly IHubClientFactory _hubClientFactory;
        private readonly EndpointId _queryEndpointId;
        private readonly IEventSerializer<Event<TCommand>> _serializer;

        public DomainCommandHandlerAdapter(
            EndpointId queryEndpointId,
            IDomainCommandHandler<TCommand> handler,
            IHubClientFactory hubClientFactory,
            IEventSerializer<Event<TCommand>> serializer)
        {
            _queryEndpointId = queryEndpointId;
            _handler = handler;
            _hubClientFactory = hubClientFactory;
            _serializer = serializer;
        }

        public TypeId TypeId => _serializer.TypeId;

        public async Task HandleAsync(Event @event)
        {
            var command = _serializer.Deserialize(@event);
            await _handler.HandleAsync(
                new DomainCommandContext(_queryEndpointId, command.CreateEventContext(_hubClientFactory)),
                command.Data);
        }
    }
}