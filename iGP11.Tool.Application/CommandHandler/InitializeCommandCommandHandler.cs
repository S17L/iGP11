using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;
using iGP11.Tool.Shared.Event;

using ApplicationModel = iGP11.Tool.Shared.Model;
using ApplicationSettings = iGP11.Tool.Shared.Model.ApplicationSettings.ApplicationSettings;
using InjectionSettings = iGP11.Tool.Shared.Model.InjectionSettings.InjectionSettings;
using TextureManagementSettings = iGP11.Tool.Shared.Model.TextureManagementSettings.TextureManagementSettings;

namespace iGP11.Tool.Application.CommandHandler
{
    public class InitializeCommandCommandHandler : IDomainCommandHandler<InitializeCommand>
    {
        private readonly IApplicationSettingsRepository _applicationSettingsRepository;
        private readonly IInjectionSettingsRepository _injectionSettingsRepository;
        private readonly ITextureManagementSettingsRepository _textureManagementSettingsRepository;
        private readonly IUsageStatisticsRepository _usageStatisticsRepository;

        public InitializeCommandCommandHandler(
            IApplicationSettingsRepository applicationSettingsRepository,
            IInjectionSettingsRepository injectionSettingsRepository,
            ITextureManagementSettingsRepository textureManagementSettingsRepository,
            IUsageStatisticsRepository usageStatisticsRepository)
        {
            _applicationSettingsRepository = applicationSettingsRepository;
            _injectionSettingsRepository = injectionSettingsRepository;
            _textureManagementSettingsRepository = textureManagementSettingsRepository;
            _usageStatisticsRepository = usageStatisticsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, InitializeCommand command)
        {
            var applicationSettings = await _applicationSettingsRepository.LoadAsync();
            var injectionSettings = await _injectionSettingsRepository.LoadAllAsync();
            var defaultId = await _injectionSettingsRepository.LoadDefaultAsync();
            var textureManagementSettings = await _textureManagementSettingsRepository.LoadAsync();
            var usageStatistics = await _usageStatisticsRepository.LoadAsync();

            var @event = new InitializeEvent(
                applicationSettings.Map<ApplicationSettings>(),
                injectionSettings.Map<IEnumerable<InjectionSettings>>(),
                defaultId?.Value,
                textureManagementSettings.Map<TextureManagementSettings>(),
                usageStatistics.Map<ApplicationModel.UsageStatistics>());

            await context.PublishAsync(@event);
        }
    }
}