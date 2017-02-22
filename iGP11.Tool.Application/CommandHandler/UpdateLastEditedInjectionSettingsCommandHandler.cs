using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateLastEditedInjectionSettingsCommandHandler : IDomainCommandHandler<UpdateLastEditedInjectionSettingsCommand>
    {
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;

        public UpdateLastEditedInjectionSettingsCommandHandler(IInjectionSettingsRepository injectionSettingsRepository)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateLastEditedInjectionSettingsCommand command)
        {
            await _injectionSettingsRepository.ChangeDefaultAsync(command.Id);
            await context.PublishAsync(new LastEditedInjectionSettingsUpdatedEvent(command.Id));
        }
    }
}