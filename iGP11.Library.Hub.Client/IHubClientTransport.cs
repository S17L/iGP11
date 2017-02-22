using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IHubClientTransport
    {
        void Connect(IHubClientTransportListener hubClientTransportListener, EndpointId id);

        Task DeliverAsync(HubClientEvent hubClientEvent);

        void Disconnect(EndpointId id);

        bool IsConnected(EndpointId id);

        void ListenFor(EndpointId id, TypeId typeId);
    }
}