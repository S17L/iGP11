using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.Hub.Client.Exceptions;
using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public sealed class HubClient : IHubClient,
                                    IHubClientTransportListener
    {
        private readonly IEventSerializerFactory _defaultSerializerFactory;
        private readonly IHubClientTransport _hubClientTransport;
        private readonly ICollection<Listener> _listeners = new List<Listener>();
        private readonly object _lock = new object();

        public HubClient(
            EndpointId id,
            IHubClientTransport hubClientTransport,
            IEventSerializerFactory defaultSerializerFactory)
        {
            Id = id;

            _defaultSerializerFactory = defaultSerializerFactory;
            _hubClientTransport = hubClientTransport;
        }

        public EndpointId Id { get; }

        public bool IsConnected => _hubClientTransport.IsConnected(Id);

        public async Task DeliverAsync(Event @event)
        {
            var listener = GetListener(@event.TypeId);
            if (listener == null)
            {
                throw new ListenerNotFoundException($"listener for event: {@event} in endpoint id: {Id} was not found");
            }

            await listener.DeliverAsync(@event);
        }

        public void Dispose()
        {
            IEnumerable<IDisposable> listeners;
            lock (_lock)
            {
                listeners = _listeners.ToArray();
            }

            foreach (var listener in listeners)
            {
                listener.Dispose();
            }

            _hubClientTransport.Disconnect(Id);
        }

        public IListener ListenFor(IListenerHandler handler)
        {
            var listener = new Listener(this, handler);
            lock (_lock)
            {
                _listeners.Add(listener);
            }

            _hubClientTransport.ListenFor(Id, handler.TypeId);

            return listener;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, PublishSettings<TEvent> settings = null)
        {
            var serializer = settings?.Serializer ?? _defaultSerializerFactory.Create<TEvent>();
            var hubClientEvent = new HubClientEvent(serializer.Serialize(@event), settings?.DestinationEndpointId);

            await _hubClientTransport.DeliverAsync(hubClientEvent);
        }

        internal void Connect()
        {
            _hubClientTransport.Connect(this, Id);
        }

        internal void Drop(Listener listener)
        {
            lock (_lock)
            {
                _listeners.Remove(listener);
            }
        }

        private Listener GetListener(TypeId typeId)
        {
            lock (_lock)
            {
                return _listeners.FirstOrDefault(listener => listener.TypeId == typeId);
            }
        }
    }
}