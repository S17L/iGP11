using System;
using System.Linq;
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
using iGP11.Tool.Domain.Model.GameSettings;

namespace iGP11.Tool.Application
{
    public class GameSettingsWatchableProcess : IWatchableProcess
    {
        private readonly ComponentAssembler _assembler;
        private readonly string _communicationAddress;
        private readonly ushort _communicationPort;
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IInjectionService _injectionService;
        private readonly ILogger _logger;
        private readonly Plugins _plugins;

        private Game _game;

        public GameSettingsWatchableProcess(
            AggregateId gameId,
            Plugins plugins,
            string communicationAddress,
            ushort communicationPort,
            ComponentAssembler assembler,
            IDirectoryRepository directoryRepository,
            IGameRepository gamesRepository,
            IInjectionService injectionService,
            ILogger logger)
        {
            GameId = gameId;
            _plugins = plugins;
            _communicationAddress = communicationAddress;
            _communicationPort = communicationPort;
            _assembler = assembler;
            _directoryRepository = directoryRepository;
            _gameRepository = gamesRepository;
            _injectionService = injectionService;
            _logger = logger;
        }

        public string FilePath => _game?.FilePath;

        public AggregateId GameId { get; }

        public async Task InitializeAsync()
        {
            _game = await _gameRepository.LoadAsync(GameId);
        }

        public void OnStarted()
        {
            if (_injectionService.IsProxyLoaded(_game.FilePath, _plugins.Proxy))
            {
                return;
            }

            var gameProfile = _game.Profiles.SingleOrDefault(profile => profile.Id == _game.ProfileId);
            if (gameProfile == null)
            {
                _logger.Log(LogLevel.Error, $"game profile could not be found for: {FilePath}");
                return;
            }

            var tokenReplacer = new TokenReplacer(new ApplicationFilePathTokenReplacingPolicy(() => _game.FilePath));
            var component = _assembler.Assemble(gameProfile, new AssemblingContext(FormType.None, tokenReplacer));
            component.Tokenize();

            if (!component.IsValid)
            {
                _logger.Log(LogLevel.Error, $"game profile for: {FilePath} are not valid; skipping...");
                return;
            }

            try
            {
                foreach (var directoryPath in component.GetDirectoryPaths())
                {
                    _logger.Log(LogLevel.Information, $"demanding access to: {directoryPath}...");
                    _directoryRepository.DemandAccess(directoryPath);
                }
            }
            catch (SecurityException)
            {
                _logger.Log(LogLevel.Error, $"insufficient permissions to: {FilePath}");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                _logger.Log(LogLevel.Error, $"insufficient permissions to: {FilePath}");
                return;
            }

            var settings = new InjectableProxySettings
            {
                CommunicationAddress = _communicationAddress,
                CommunicationPort = _communicationPort,
                Direct3D11PluginPath = _plugins.Direct3D11,
                GameProfile = gameProfile,
                GameFilePath = _game.FilePath
            };

            Inject(settings).Wait();
        }

        public void OnTerminated()
        {
        }

        private async Task Inject(InjectableProxySettings injectProxySettings)
        {
            _logger.Log(LogLevel.Information, $"injection started for: {FilePath}...");
            await new SynchronizationContextRemover();

            var directory = await _directoryRepository.LoadAsync(injectProxySettings.GameProfile.ProxyDirectoryPath);
            directory.AddFile(_plugins.ProxySettingsFileName, injectProxySettings.Serialize());

            _logger.Log(LogLevel.Information, $"creating proxy settings file at: {FilePath}...");
            await _directoryRepository.SaveAsync(directory);

            _logger.Log(LogLevel.Information, $"attempting to inject proxy dll into: {FilePath}...");
            var status = _injectionService.Inject(_game.FilePath, _plugins.Proxy);
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