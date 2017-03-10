using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateGameProfileCommandHandler : IDomainCommandHandler<UpdateGameProfileCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameSettingsProcessWatcher _processWatcher;

        public UpdateGameProfileCommandHandler(
            IGameRepository gameRepository,
            GameSettingsProcessWatcher processWatcher)
        {
            _gameRepository = gameRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateGameProfileCommand command)
        {
            var gameProfile = command.GameProfile.Map<GameProfile>();
            var game = await _gameRepository.LoadByGameProfileId(gameProfile.Id);

            var profile = game.AddGameProfile(
                command.GameProfile.Id,
                command.GameProfile.Name,
                command.GameProfile.ProxyDirectoryPath,
                command.GameProfile.LogsDirectoryPath,
                command.GameProfile.PluginType,
                command.GameProfile.Direct3D11Settings.Map<Direct3D11Settings>());

            await _gameRepository.SaveAsync(game);
            await _processWatcher.WatchAsync(game.Id);

            await context.PublishAsync(new GameProfileUpdatedEvent(profile.Map<Shared.Model.GameSettings.GameProfile>()));
        }
    }
}