using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class RemoveGameProfileCommandHandler : IDomainCommandHandler<RemoveGameProfileCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameSettingsProcessWatcher _processWatcher;

        public RemoveGameProfileCommandHandler(
            IGameRepository gameRepository,
            GameSettingsProcessWatcher processWatcher)
        {
            _gameRepository = gameRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, RemoveGameProfileCommand command)
        {
            var game = await _gameRepository.LoadByGameProfileId(command.Id);
            game.RemoveGameProfile(command.Id);

            await _gameRepository.SaveAsync(game);
            await _processWatcher.UnwatchAsync(command.Id);

            await context.PublishAsync(new GameProfileRemovedEvent(game.ProfileId, command.Id));
        }
    }
}