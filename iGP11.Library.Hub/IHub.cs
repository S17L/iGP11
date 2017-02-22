using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub
{
    public interface IHub
    {
        void Acquire(EndpointId id);

        Task DeliverAsync(HubClientEvent hubClientEvent);

        void Drop(EndpointId id);

        void ListenFor(EndpointId id, TypeId typeId);
    }
}