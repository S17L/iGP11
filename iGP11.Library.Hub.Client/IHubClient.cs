using System;
using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IHubClient : IDisposable
    {
        EndpointId Id { get; }

        bool IsConnected { get; }

        IListener ListenFor(IListenerHandler handler);

        Task PublishAsync<TEvent>(TEvent @event, PublishSettings<TEvent> settings = null);
    }
}