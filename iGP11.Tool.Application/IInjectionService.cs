using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application
{
    public interface IInjectionService
    {
        InjectionResult Inject(string applicationFilePath, string proxyFilePath);

        bool IsProcessRunning(string applicationFilePath);

        bool IsProxyLoaded(string applicationFilePath, string proxyFilePath);
    }
}