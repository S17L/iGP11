using System;
using System.Security;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Application.Model;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Notification;

using ApplicationStartedEvent = iGP11.Tool.Shared.Event.ApplicationStartedEvent;

namespace iGP11.Tool.Application.CommandHandler
{
    public class StartApplicationCommandHandler : IDomainCommandHandler<StartApplicationCommand>
    {
        private readonly ComponentAssembler _assembler;
        private readonly string _communicationAddress;
        private readonly ushort _communicationPort;
        private readonly string _direct3D11PluginPath;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IInjectionService _injectionService;
        private readonly IInjectionSettingsRepository _injectionSettingRepository;
        private readonly string _proxyFilePath;
        private readonly string _proxySettingsFileName;

        public StartApplicationCommandHandler(
            string proxyFilePath,
            string proxySettingsFileName,
            string communicationAddress,
            ushort communicationPort,
            string direct3D11PluginPath,
            ComponentAssembler assembler,
            IDirectoryRepository directoryRepository,
            IInjectionService injectionService,
            IInjectionSettingsRepository injectionSettingsRepository)
        {
            _proxyFilePath = proxyFilePath;
            _proxySettingsFileName = proxySettingsFileName;
            _communicationAddress = communicationAddress;
            _communicationPort = communicationPort;
            _direct3D11PluginPath = direct3D11PluginPath;
            _assembler = assembler;
            _directoryRepository = directoryRepository;
            _injectionService = injectionService;
            _injectionSettingRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, StartApplicationCommand command)
        {
            var injectionSettings = await _injectionSettingRepository.LoadAsync(command.Id);

            if (_injectionService.IsProxyLoaded(injectionSettings.ApplicationFilePath, _proxyFilePath))
            {
                await context.PublishAsync(
                    new ApplicationStartedEvent(
                        injectionSettings.ApplicationFilePath,
                        InjectionStatus.PluginAlreadyLoaded));

                return;
            }

            var tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => injectionSettings.ApplicationFilePath));
            var component = _assembler.Assemble(injectionSettings, new AssemblingContext(FormType.None, tokenReplacer));
            component.Tokenize();

            try
            {
                foreach (var directoryPath in component.GetDirectoryPaths())
                {
                    _directoryRepository.DemandAccess(directoryPath);
                }
            }
            catch (SecurityException)
            {
                await context.EmitAsync(new ErrorOccuredEvent(new Localizable("InsufficientPermissions")));
                return;
            }
            catch (UnauthorizedAccessException)
            {
                await context.EmitAsync(new ErrorOccuredEvent(new Localizable("InsufficientPermissions")));
                return;
            }

            var settings = new InjectProxySettings
            {
                CommunicationAddress = _communicationAddress,
                CommunicationPort = _communicationPort,
                Direct3D11PluginPath = _direct3D11PluginPath,
                InjectionSettings = injectionSettings
            };

            var directory = await _directoryRepository.LoadAsync(settings.InjectionSettings.ConfigurationDirectoryPath);
            directory.AddFile(_proxySettingsFileName, settings.Serialize());

            await _directoryRepository.SaveAsync(directory);

            var status = _injectionService.Inject(settings.InjectionSettings.ApplicationFilePath, _proxyFilePath)?.Status == 0
                             ? InjectionStatus.Completed
                             : InjectionStatus.Failed;

            await context.PublishAsync(
                new ApplicationStartedEvent(
                    injectionSettings.ApplicationFilePath,
                    status));
        }
    }
}