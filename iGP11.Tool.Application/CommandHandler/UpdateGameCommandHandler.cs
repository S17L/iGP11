using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateGameCommandHandler : IDomainCommandHandler<UpdateGameCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly GameSettingsProcessWatcher _processWatcher;

        public UpdateGameCommandHandler(
            IGameRepository gameRepository,
            GameSettingsProcessWatcher processWatcher)
        {
            _gameRepository = gameRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateGameCommand command)
        {
            var game = await _gameRepository.LoadAsync(command.Id);
            game.ChangeName(command.Name);
            game.ChangeFilePath(command.FilePath);
            
            await _gameRepository.SaveAsync(game);
            await _processWatcher.WatchAsync(game.Id);

            await context.PublishAsync(new GameUpdatedEvent(game.Id, game.Name, game.FilePath));
        }
    }
}