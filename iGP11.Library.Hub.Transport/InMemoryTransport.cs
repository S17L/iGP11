using System.Collections.Concurrent;
using System.Threading.Tasks;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Exceptions;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Transport
{
    public class InMemoryTransport : IHubClientTransport,
                                     IHubTransport
    {
        private readonly ConcurrentDictionary<EndpointId, IHubClientTransportListener> _hubClientTransportListeners = new ConcurrentDictionary<EndpointId, IHubClientTransportListener>();
        private IHub _hub;

        void IHubClientTransport.Connect(IHubClientTransportListener hubClientTransportListener, EndpointId id)
        {
            if (_hub == null)
            {
                throw new HubNotFoundException("hub not connected");
            }

            _hub.Acquire(id);
            _hubClientTransportListeners[id] = hubClientTransportListener;
        }

        async Task IHubClientTransport.DeliverAsync(HubClientEvent hubClientEvent)
        {
            await _hub.DeliverAsync(hubClientEvent);
        }

        void IHubClientTransport.Disconnect(EndpointId id)
        {
            IHubClientTransportListener hubClientTransportListener;
            _hub.Drop(id);
            _hubClientTransportListeners.TryRemove(id, out hubClientTransportListener);
        }

        bool IHubClientTransport.IsConnected(EndpointId id)
        {
            return _hubClientTransportListeners.ContainsKey(id);
        }

        void IHubClientTransport.ListenFor(EndpointId id, TypeId typeId)
        {
            _hub.ListenFor(id, typeId);
        }

        void IHubTransport.Connect(IHub hub)
        {
            _hub = hub;
        }

        async Task IHubTransport.DeliverAsync(Event @event, EndpointId id)
        {
            await _hubClientTransportListeners[id].DeliverAsync(@event);
        }
    }
}