using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateLastEditedGameProfileCommandHandler : IDomainCommandHandler<UpdateLastEditedGameProfileCommand>
    {
        private readonly IGameRepository _gameRepository;

        public UpdateLastEditedGameProfileCommandHandler(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateLastEditedGameProfileCommand command)
        {
            await _gameRepository.ChangeGameProfileAsync(command.Id);
            await context.PublishAsync(new LastEditedGameProfileUpdatedEvent(command.Id));
        }
    }
}