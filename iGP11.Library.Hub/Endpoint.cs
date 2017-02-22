using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library.Hub.Model;
using iGP11.Library.Hub.Queue;
using iGP11.Library.Hub.Shared;
using iGP11.Library.Scheduler;

namespace iGP11.Library.Hub
{
    public sealed class Endpoint : IDisposable,
                                   IEventHandler<EndpointEvent>
    {
        private readonly IHub _hub;
        private readonly IHubTransport _hubTransport;
        private readonly object _lock = new object();
        private readonly EventQueue<EndpointEvent> _queue;
        private readonly IAsynchronousScheduler _scheduler;
        private readonly ICollection<TypeId> _typeIds = new List<TypeId>();

        public Endpoint(
            IHub hub,
            EndpointId id,
            IEventRepository<EndpointEvent> endpointEventRepository,
            IHubTransport hubTransport,
            ILogger logger,
            IAsynchronousScheduler scheduler)
        {
            _hub = hub;
            _hubTransport = hubTransport;
            _scheduler = scheduler;

            Id = id;

            _queue = new EventQueue<EndpointEvent>(this, endpointEventRepository, new NoEventSchedulingPolicy(), logger);
            _scheduler.Subscribe(_queue);
        }

        public EndpointId Id { get; }

        public void Dispose()
        {
            _scheduler.Unsubscribe(_queue);
        }

        public async Task HandleAsync(EndpointEvent @event)
        {
            await _hubTransport.DeliverAsync(@event.Event, Id);
        }

        public void ListenFor(TypeId typeId)
        {
            lock (_lock)
            {
                _typeIds.Add(typeId);
            }
        }

        internal void Deliver(EndpointEvent @event)
        {
            _queue.Enqueue(@event);
        }

        internal async Task DeliverAsync(HubClientEvent hubClientEvent)
        {
            await _hub.DeliverAsync(hubClientEvent);
        }

        internal bool IsApplicable(TypeId typeId)
        {
            lock (_lock)
            {
                return _typeIds.Contains(typeId);
            }
        }
    }
}