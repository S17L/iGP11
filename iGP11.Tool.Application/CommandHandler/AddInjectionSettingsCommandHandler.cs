using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Event;

using SharedModel = iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.CommandHandler
{
    public class AddInjectionSettingsCommandHandler : IDomainCommandHandler<AddInjectionSettingsCommand>
    {
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;

        public AddInjectionSettingsCommandHandler(IInjectionSettingsRepository injectionSettingsRepository)
        {
            _injectionSettingsRepository = injectionSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, AddInjectionSettingsCommand command)
        {
            var template = await _injectionSettingsRepository.LoadAsync(command.BasedOnInjectionSettingsId);
            var injectionSettings = new InjectionSettings(
                AggregateId.Generate(),
                command.Name,
                template.ApplicationFilePath,
                template.ConfigurationDirectoryPath,
                template.LogsDirectoryPath,
                template.PluginType,
                template.EstablishCommunication,
                template.Direct3D11Settings);

            await _injectionSettingsRepository.SaveAsync(injectionSettings);
            await context.PublishAsync(new InjectionSettingsAddedEvent(injectionSettings.Map<Shared.Model.InjectionSettings.InjectionSettings>()));
        }
    }
}