using System;
using System.Collections.Generic;

using iGP11.Library.Hub.Client;
using iGP11.Library.Hub.Client.Action;
using iGP11.Library.Hub.Shared;
using iGP11.Library.Scheduler;

namespace iGP11.Library.DDD.Action
{
    public sealed class DomainCommandHubHost : IDisposable
    {
        private const int Interval = 1000;

        private readonly ICollection<object> _adapters = new List<object>();
        private readonly EndpointId _commandEndpointId;
        private readonly IHubClientFactory _hubClientFactory;
        private readonly EndpointId _queryEndpointId;
        private readonly IScheduler _scheduler;

        private IHubClient _hubClient;

        public DomainCommandHubHost(
            EndpointId commandEndpointId,
            EndpointId queryEndpointId,
            IHubClientFactory hubClientFactory)
        {
            _commandEndpointId = commandEndpointId;
            _queryEndpointId = queryEndpointId;
            _hubClientFactory = hubClientFactory;
            _scheduler = new BlockingScheduler(KeepAlive, Interval);
        }

        public void Dispose()
        {
            _scheduler.Stop();
            _scheduler.Dispose();
        }

        public void ListenFor<TCommand>(IDomainCommandHandler<TCommand> handler, IEventSerializer<Event<TCommand>> serializer)
        {
            var adapter = new DomainCommandHandlerAdapter<TCommand>(
                _queryEndpointId,
                new TransactionalCommandHandler<TCommand>(handler),
                _hubClientFactory,
                serializer);

            _adapters.Add(adapter);
        }

        public void Start()
        {
            _scheduler.Start();
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
            _hubClient = _hubClientFactory.Create(_commandEndpointId);

            foreach (dynamic adapter in _adapters)
            {
                _hubClient.ListenFor(adapter);
            }
        }
    }
}