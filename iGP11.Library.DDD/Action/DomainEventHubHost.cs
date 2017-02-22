using System;
using System.Collections.Generic;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;
using iGP11.Library.Scheduler;

namespace iGP11.Library.DDD.Action
{
    public sealed class DomainEventHubHost : IDisposable
    {
        private const int Interval = 1000;

        private readonly ICollection<object> _adapters = new List<object>();
        private readonly IHubClientFactory _hubClientFactory;
        private readonly EndpointId _queryEndpointId;
        private readonly IScheduler _scheduler;

        private IHubClient _hubClient;

        public DomainEventHubHost(
            EndpointId queryEndpointId,
            IHubClientFactory hubClientFactory)
        {
            _queryEndpointId = queryEndpointId;
            _hubClientFactory = hubClientFactory;
            _scheduler = new BlockingScheduler(KeepAlive, Interval);
            _scheduler.Start();
        }

        public void Dispose()
        {
            _scheduler.Stop();
            _scheduler.Dispose();
            _hubClient?.Dispose();
        }

        public void ListenFor<TEvent>(IDomainEventHandler<TEvent> handler, IEventSerializer<Event<TEvent>> serializer)
        {
            var adapter = new DomainEventHandlerAdapter<TEvent>(
                handler,
                _hubClientFactory,
                serializer);

            _adapters.Add(adapter);
        }

        public void Start()
        {
            Reinitialize();
        }

        private void KeepAlive()
        {
            if ((_hubClient != null) && !_hubClient.IsConnected)
            {
                Reinitialize();
            }
        }

        private void Reinitialize()
        {
            _hubClient?.Dispose();
            _hubClient = _hubClientFactory.Create(_queryEndpointId);

            foreach (dynamic adapter in _adapters)
            {
                _hubClient.ListenFor(adapter);
            }
        }
    }
}