using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class UpdateApplicationSettingsCommandHandler : IDomainCommandHandler<UpdateApplicationSettingsCommand>
    {
        private readonly IApplicationSettingsRepository _applicationSettingsRepository;

        public UpdateApplicationSettingsCommandHandler(IApplicationSettingsRepository applicationSettingsRepository)
        {
            _applicationSettingsRepository = applicationSettingsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, UpdateApplicationSettingsCommand command)
        {
            var applicationSettings = await _applicationSettingsRepository.LoadAsync();
            applicationSettings.ApplicationCommunicationPort = command.ApplicationCommunicationPort;
            applicationSettings.ProxyCommunicationPort = command.ProxyCommunicationPort;

            await _applicationSettingsRepository.SaveAsync(applicationSettings);
            await context.PublishAsync(new ApplicationSettingsUpdatedEvent(command.ApplicationCommunicationPort, command.ProxyCommunicationPort));
        }
    }
}