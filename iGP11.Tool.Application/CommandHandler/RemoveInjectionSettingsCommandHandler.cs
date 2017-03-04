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
        private readonly InjectionSettingsProcessWatcher _processWatcher;

        public RemoveInjectionSettingsCommandHandler(
            IInjectionSettingsRepository injectionSettingsRepository,
            InjectionSettingsProcessWatcher processWatcher)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, RemoveInjectionSettingsCommand command)
        {
            await _injectionSettingsRepository.RemoveAsync(command.Id);
            await _processWatcher.UnwatchAsync(command.Id);

            await context.PublishAsync(new InjectionSettingsRemovedEvent(command.Id));
        }
    }
}