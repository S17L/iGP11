using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Model;

using SharedEvent = iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class StartApplicationCommandHandler : IDomainCommandHandler<StartApplicationCommand>
    {
        private readonly IInjectionService _injectionService;
        private readonly IInjectionSettingsRepository _injectionSettingRepository;
        private readonly Plugins _plugins;

        public StartApplicationCommandHandler(
            Plugins plugins,
            IInjectionService injectionService,
            IInjectionSettingsRepository injectionSettingsRepository)
        {
            _plugins = plugins;
            _injectionService = injectionService;
            _injectionSettingRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, StartApplicationCommand command)
        {
            var injectionSettings = await _injectionSettingRepository.LoadAsync(command.Id);

            if (_injectionService.IsProxyLoaded(injectionSettings.ApplicationFilePath, _plugins.Proxy))
            {
                await context.PublishAsync(
                    new SharedEvent.ApplicationStartedEvent(
                        injectionSettings.ApplicationFilePath,
                        InjectionStatus.PluginAlreadyLoaded));

                return;
            }

            var result = _injectionService.Start(injectionSettings.ApplicationFilePath)
                             ? InjectionStatus.Completed
                             : InjectionStatus.Failed;

            await context.PublishAsync(new SharedEvent.ApplicationStartedEvent(injectionSettings.ApplicationFilePath, result));
        }
    }
}