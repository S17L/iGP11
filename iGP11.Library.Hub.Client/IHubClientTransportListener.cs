using System.Threading.Tasks;

using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IHubClientTransportListener
    {
        Task DeliverAsync(Event @event);
    }
}