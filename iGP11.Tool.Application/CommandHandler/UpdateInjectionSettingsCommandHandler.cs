using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateInjectionSettingsCommandHandler : IDomainCommandHandler<UpdateInjectionSettingsCommand>
    {
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;

        public UpdateInjectionSettingsCommandHandler(IInjectionSettingsRepository injectionSettingsRepository)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateInjectionSettingsCommand command)
        {
            var injectionSettings = command.InjectionSettings.Map<InjectionSettings>();
            await _injectionSettingsRepository.LoadAsync(injectionSettings.Id);
            await _injectionSettingsRepository.SaveAsync(injectionSettings);
            await context.PublishAsync(new InjectionSettingsUpdatedEvent(command.InjectionSettings));
        }
    }
}