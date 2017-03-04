using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.CommandHandler
{
    public class LoadProxyActivationStatusCommandHandler : IDomainCommandHandler<LoadProxyActivationStatusCommand>
    {
        private readonly IInjectionService _injectionService;
        private readonly Plugins _plugins;

        public LoadProxyActivationStatusCommandHandler(
            Plugins plugins,
            IInjectionService injectionService)
        {
            _plugins = plugins;
            _injectionService = injectionService;
        }

        public async Task HandleAsync(DomainCommandContext context, LoadProxyActivationStatusCommand command)
        {
            var status = GetActivationStatus(command.ApplicationFilePath);
            await context.PublishAsync(new ProxyActivationStatusLoadedEvent(command.ApplicationFilePath, status));
        }

        private ActivationStatus GetActivationStatus(string applicationFilePath)
        {
            if (applicationFilePath.IsNullOrEmpty() || !_injectionService.IsProcessRunning(applicationFilePath))
            {
                return ActivationStatus.NotRunning;
            }

            return !_injectionService.IsProxyLoaded(applicationFilePath, _plugins.Proxy)
                       ? ActivationStatus.Running
                       : ActivationStatus.PluginLoaded;
        }
    }
}