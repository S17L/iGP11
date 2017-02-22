using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateTextureManagementSettingsCommandHandler : IDomainCommandHandler<UpdateTextureManagementSettingsCommand>
    {
        private readonly ITextureManagementSettingsRepository _textureManagementSettingsRepository;

        public UpdateTextureManagementSettingsCommandHandler(ITextureManagementSettingsRepository textureManagementSettingsRepository)
        {
            _textureManagementSettingsRepository = textureManagementSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateTextureManagementSettingsCommand command)
        {
            var settings = command.Settings.Map<TextureManagementSettings>();

            await _textureManagementSettingsRepository.SaveAsync(settings);
            await context.PublishAsync(new TextureManagementSettingsUpdatedEvent(command.Settings));
        }
    }
}