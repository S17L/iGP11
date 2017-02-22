using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class RemoveInjectionSettingsCommandHandler : IDomainCommandHandler<RemoveInjectionSettingsCommand>
    {
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;

        public RemoveInjectionSettingsCommandHandler(IInjectionSettingsRepository injectionSettingsRepository)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, RemoveInjectionSettingsCommand command)
        {
            await _injectionSettingsRepository.RemoveAsync(command.Id);
            await context.PublishAsync(new InjectionSettingsRemovedEvent(command.Id));
        }
    }
}