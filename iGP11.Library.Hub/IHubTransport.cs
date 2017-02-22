using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub
{
    public interface IHubTransport
    {
        void Connect(IHub hub);

        Task DeliverAsync(Event @event, EndpointId id);
    }
}