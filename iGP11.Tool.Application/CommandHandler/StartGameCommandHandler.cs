using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Application.Bootstrapper;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Model;

using SharedEvent = iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class StartGameCommandHandler : IDomainCommandHandler<StarGameCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IInjectionService _injectionService;
        private readonly Plugins _plugins;

        public StartGameCommandHandler(
            Plugins plugins,
            IGameRepository gamesRepository,
            IInjectionService injectionService)
        {
            _plugins = plugins;
            _gameRepository = gamesRepository;
            _injectionService = injectionService;
        }

        public async Task HandleAsync(DomainCommandContext context, StarGameCommand command)
        {
            var game = await _gameRepository.LoadAsync(command.GameId);

            if (_injectionService.IsProxyLoaded(game.FilePath, _plugins.Proxy))
            {
                await context.PublishAsync(
                    new SharedEvent.GameStartedEvent(
                        game.FilePath,
                        GameLaunchingStatus.PluginAlreadyLoaded));

                return;
            }

            var result = _injectionService.Start(game.FilePath)
                             ? GameLaunchingStatus.Completed
                             : GameLaunchingStatus.Failed;

            await context.PublishAsync(new SharedEvent.GameStartedEvent(game.FilePath, result));
        }
    }
}