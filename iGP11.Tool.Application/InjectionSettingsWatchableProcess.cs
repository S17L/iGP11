using System;
using System.Security;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Library.DDD;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Application.Model;
using iGP11.Tool.Domain.Model.Directory;
using iGP11.Tool.Domain.Model.InjectionSettings;

namespace iGP11.Tool.Application
{
    public class InjectionSettingsWatchableProcess : IWatchableProcess
    {
        private readonly ComponentAssembler _assembler;
        private readonly string _communicationAddress;
        private readonly ushort _communicationPort;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IInjectionService _injectionService;
        private readonly IInjectionSettingsRepository _injectionSettingRepository;
        private readonly ILogger _logger;
        private readonly Plugins _plugins;

        private InjectionSettings _injectionSettings;

        public InjectionSettingsWatchableProcess(
            AggregateId id,
            Plugins plugins,
            string communicationAddress,
            ushort communicationPort,
            ComponentAssembler assembler,
            IDirectoryRepository directoryRepository,
            IInjectionService injectionService,
            IInjectionSettingsRepository injectionSettingsRepository,
            ILogger logger)
        {
            Id = id;
            _plugins = plugins;
            _communicationAddress = communicationAddress;
            _communicationPort = communicationPort;
            _assembler = assembler;
            _directoryRepository = directoryRepository;
            _injectionService = injectionService;
            _injectionSettingRepository = injectionSettingsRepository;
            _logger = logger;
        }

        public string FilePath => _injectionSettings?.ApplicationFilePath;

        public AggregateId Id { get; }

        public async Task InitializeAsync()
        {
            _injectionSettings = await _injectionSettingRepository.LoadAsync(Id);
        }

        public void OnStarted()
        {
            if (_injectionService.IsProxyLoaded(_injectionSettings.ApplicationFilePath, _plugins.Proxy))
            {
                return;
            }

            var tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => _injectionSettings.ApplicationFilePath));
            var component = _assembler.Assemble(_injectionSettings, new AssemblingContext(FormType.None, tokenReplacer));
            component.Tokenize();

            if (!component.IsValid)
            {
                _logger.Log(LogLevel.Error, $"injection settings for: {FilePath} are not valid; skipping...");
                return;
            }

            try
            {
                foreach (var directoryPath in component.GetDirectoryPaths())
                {
                    _logger.Log(LogLevel.Information, $"demanding access to: {directoryPath}...");
                    _directoryRepository.DemandAccess(directoryPath);
                }

                _logger.Log(LogLevel.Information, "directories ensured");
            }
            catch (SecurityException)
            {
                _logger.Log(LogLevel.Error, $"insufficient permissions for: {FilePath}");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Log(LogLevel.Error, $"insufficient permissions for: {FilePath}");
                return;
            }

            var settings = new InjectProxySettings
            {
                CommunicationAddress = _communicationAddress,
                CommunicationPort = _communicationPort,
                Direct3D11PluginPath = _plugins.Direct3D11,
                InjectionSettings = _injectionSettings
            };

            Inject(settings).Wait();
        }

        public void OnTerminated()
        {
        }

        private async Task Inject(InjectProxySettings injectProxySettings)
        {
            await new SynchronizationContextRemover();

            var directory = await _directoryRepository.LoadAsync(injectProxySettings.InjectionSettings.ProxyDirectoryPath);
            directory.AddFile(_plugins.ProxySettingsFileName, injectProxySettings.Serialize());

            _logger.Log(LogLevel.Information, $"creating proxy settings file at: {FilePath}...");
            await _directoryRepository.SaveAsync(directory);

            _logger.Log(LogLevel.Information, $"attempting to inject proxy dll into: {FilePath}...");
            var status = _injectionService.Inject(injectProxySettings.InjectionSettings.ApplicationFilePath, _plugins.Proxy);
            if (status?.Status == 0)
            {
                _logger.Log(LogLevel.Information, $"proxy dll injected into: {FilePath}");
            }
            else
            {
                _logger.Log(LogLevel.Error, $"proxy dll could not be injected into: {FilePath}; failed with status: {status}");
            }
        }
    }
}