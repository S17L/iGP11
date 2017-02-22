using System.Threading.Tasks;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.DDD.Action
{
    public class DomainCommandContext
    {
        private readonly FeedbackEventContext _context;
        private readonly EndpointId _queryEndpointId;

        public DomainCommandContext(
            EndpointId queryEndpointId,
            FeedbackEventContext context)
        {
            _queryEndpointId = queryEndpointId;
            _context = context;
        }

        public async Task EmitAsync<TEvent>(TEvent @event)
        {
            await _context.EmitAsync(@event);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, IEventSerializer<Event<TEvent>> serializer = null)
        {
            var settings = new PublishSettings<Event<TEvent>>
            {
                DestinationEndpointId = _queryEndpointId,
                Serializer = serializer
            };

            await _context.PublishAsync(@event, settings);
        }

        public override string ToString()
        {
            return _context.ToString();
        }
    }
}