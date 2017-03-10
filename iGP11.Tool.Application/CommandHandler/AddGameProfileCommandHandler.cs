using System.Linq;
using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Event;
using iGP11.Tool.Shared.Notification;

using GameProfile = iGP11.Tool.Shared.Model.GameSettings.GameProfile;

namespace iGP11.Tool.Application.CommandHandler
{
    public class AddGameProfileCommandHandler : IDomainCommandHandler<AddGameProfileCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameSettingsProcessWatcher _processWatcher;

        public AddGameProfileCommandHandler(
            IGameRepository gameRepository,
            GameSettingsProcessWatcher processWatcher)
        {
            _gameRepository = gameRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, AddGameProfileCommand command)
        {
            var game = await _gameRepository.LoadAsync(command.GameId);
            if (game == null)
            {
                await context.EmitAsync(new ErrorOccuredNotification());
                return;
            }

            var gameProfileTemplate = game.Profiles.SingleOrDefault(entity => entity.Id == (AggregateId)command.BasedOnProfileId);
            if (gameProfileTemplate == null)
            {
                await context.EmitAsync(new ErrorOccuredNotification());
                return;
            }

            var gameProfile = game.AddGameProfile(
                AggregateId.Generate(),
                command.Name,
                gameProfileTemplate.ProxyDirectoryPath,
                gameProfileTemplate.LogsDirectoryPath,
                gameProfileTemplate.PluginType,
                gameProfileTemplate.Direct3D11Settings);

            await _gameRepository.SaveAsync(game);
            await _processWatcher.WatchAsync(game.Id);

            var @event = new GameProfileAddedEvent(
                game.Id,
                game.ProfileId,
                gameProfile.Map<GameProfile>());

            await context.PublishAsync(@event);
        }
    }
}