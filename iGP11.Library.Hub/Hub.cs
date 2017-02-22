using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.Hub.Exceptions;
using iGP11.Library.Hub.Model;
using iGP11.Library.Hub.Queue;
using iGP11.Library.Hub.Shared;
using iGP11.Library.Scheduler;

namespace iGP11.Library.Hub
{
    public sealed class Hub : IDisposable,
                              IEventHandler<HubEvent>,
                              IHub
    {
        private readonly IEventRepository<EndpointEvent> _endpointEventRepository;
        private readonly ICollection<Endpoint> _endpoints = new List<Endpoint>();
        private readonly IEventRepository<HubEvent> _hubEventRepository;
        private readonly IHubTransport _hubTransport;
        private readonly object _lock = new object();
        private readonly ILogger _logger;

        private readonly EventQueue<HubEvent> _queue;
        private readonly IAsynchronousScheduler _scheduler;

        public Hub(
            IEventRepository<HubEvent> hubEventRepository,
            IEventRepository<EndpointEvent> endpointEventRepository,
            IHubTransport hubTransport,
            ILogger logger)
        {
            _hubEventRepository = hubEventRepository;
            _endpointEventRepository = endpointEventRepository;
            _hubTransport = hubTransport;
            _logger = logger;

            _scheduler = new AsynchronousScheduler(_logger);
            _queue = new EventQueue<HubEvent>(this, _hubEventRepository, new NoEventSchedulingPolicy(), _logger);
            _scheduler.Subscribe(_queue);
            _hubTransport.Connect(this);
            _scheduler.Start();
        }

        public void Acquire(EndpointId id)
        {
            lock (_lock)
            {
                if (_endpoints.Any(endpoint => endpoint.Id == id))
                {
                    throw new DuplicatedEndpointException($"endpoint with id: {id} already exists");
                }

                _endpoints.Add(new Endpoint(this, id, _endpointEventRepository, _hubTransport, _logger, _scheduler));
            }
        }

        public async Task DeliverAsync(HubClientEvent hubClientEvent)
        {
            var endpointIds = hubClientEvent.RecipientId.HasValue
                                  ? new[] { hubClientEvent.RecipientId.Value }
                                  : GetEndpointIds(hubClientEvent.Event.TypeId);

            var hubEvent = new HubEvent(hubClientEvent.Event, endpointIds);
            await _hubEventRepository.UpdateAsync(hubEvent);
            _queue.Enqueue(hubEvent);
        }

        public void Dispose()
        {
            _scheduler.Unsubscribe(_queue);
            _scheduler.Dispose();

            IEnumerable<Endpoint> endpoints;
            lock (_lock)
            {
                endpoints = _endpoints.ToArray();
            }

            foreach (var endpoint in endpoints)
            {
                endpoint.Dispose();
            }
        }

        public void Drop(EndpointId id)
        {
            lock (_lock)
            {
                _endpoints.Remove(endpoint => endpoint.Id == id);
            }
        }

        public async Task HandleAsync(HubEvent @event)
        {
            var events = @event.EndpointIds
                .Select(endpointId => new EndpointEvent(@event.EventId, endpointId, @event.Event))
                .ToArray();

            foreach (var endpointEvent in events)
            {
                await _endpointEventRepository.UpdateAsync(endpointEvent);
                FindEndpointById(endpointEvent.EndpointId)?.Deliver(endpointEvent);
            }
        }

        public void ListenFor(EndpointId id, TypeId typeId)
        {
            var endpoint = FindEndpointById(id);
            if (endpoint == null)
            {
                throw new EndpointNotFoundException($"endpoint with id: {id} was not found");
            }

            endpoint.ListenFor(typeId);
        }

        private Endpoint FindEndpointById(EndpointId id)
        {
            lock (_lock)
            {
                return _endpoints.SingleOrDefault(endpoint => endpoint.Id == id);
            }
        }

        private IEnumerable<EndpointId> GetEndpointIds(TypeId typeId)
        {
            lock (_lock)
            {
                return _endpoints.Where(endpoint => endpoint.IsApplicable(typeId)).Select(connection => connection.Id).ToArray();
            }
        }
    }
}