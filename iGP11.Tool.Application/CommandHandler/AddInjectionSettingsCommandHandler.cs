using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class AddInjectionSettingsCommandHandler : IDomainCommandHandler<AddInjectionSettingsCommand>
    {
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;
        private readonly InjectionSettingsProcessWatcher _processWatcher;

        public AddInjectionSettingsCommandHandler(
            IInjectionSettingsRepository injectionSettingsRepository,
            InjectionSettingsProcessWatcher processWatcher)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, AddInjectionSettingsCommand command)
        {
            var template = await _injectionSettingsRepository.LoadAsync(command.BasedOnInjectionSettingsId);
            var injectionSettings = new InjectionSettings(
                AggregateId.Generate(),
                command.Name,
                template.ApplicationFilePath,
                template.ProxyDirectoryPath,
                template.LogsDirectoryPath,
                template.PluginType,
                template.Direct3D11Settings);

            await _injectionSettingsRepository.SaveAsync(injectionSettings);
            await _processWatcher.WatchAsync(injectionSettings.Id);

            await context.PublishAsync(new InjectionSettingsAddedEvent(injectionSettings.Map<Shared.Model.InjectionSettings.InjectionSettings>()));
        }
    }
}