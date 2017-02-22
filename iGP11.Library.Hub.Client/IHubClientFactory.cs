using iGP11.Library.Hub.Shared;

namespace iGP11.Library.Hub.Client
{
    public interface IHubClientFactory
    {
        IHubClient Create(EndpointId id);
    }
}