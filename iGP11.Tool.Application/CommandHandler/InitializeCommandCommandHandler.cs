using System.Collections.Generic;
using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.ApplicationSettings;
using iGP11.Tool.Domain.Model.GameSettings;
using iGP11.Tool.Domain.Model.TextureManagementSettings;
using iGP11.Tool.Domain.Model.UsageStatistics;
using iGP11.Tool.Shared.Event;

using ApplicationSettings = iGP11.Tool.Shared.Model.ApplicationSettings.ApplicationSettings;
using Game = iGP11.Tool.Shared.Model.GameSettings.Game;
using SharedModel = iGP11.Tool.Shared.Model;
using TextureManagementSettings = iGP11.Tool.Shared.Model.TextureManagementSettings.TextureManagementSettings;

namespace iGP11.Tool.Application.CommandHandler
{
    public class InitializeCommandCommandHandler : IDomainCommandHandler<InitializeCommand>
    {
        private readonly IApplicationSettingsRepository _applicationSettingsRepository;
        private readonly IGameRepository _gameRepository;
        private readonly GameSettingsProcessWatcher _processWatcher;
        private readonly ITextureManagementSettingsRepository _textureManagementSettingsRepository;
        private readonly IUsageStatisticsRepository _usageStatisticsRepository;

        public InitializeCommandCommandHandler(
            IApplicationSettingsRepository applicationSettingsRepository,
            IGameRepository gameRepository,
            ITextureManagementSettingsRepository textureManagementSettingsRepository,
            IUsageStatisticsRepository usageStatisticsRepository,
            GameSettingsProcessWatcher processWatcher)
        {
            _applicationSettingsRepository = applicationSettingsRepository;
            _gameRepository = gameRepository;
            _textureManagementSettingsRepository = textureManagementSettingsRepository;
            _usageStatisticsRepository = usageStatisticsRepository;
            _processWatcher = processWatcher;
        }

        public async Task HandleAsync(DomainCommandContext context, InitializeCommand command)
        {
            var applicationSettings = await _applicationSettingsRepository.LoadAsync();
            var games = await _gameRepository.LoadAllAsync();
            var gameProfileId = await _gameRepository.LoadGameProfileIdAsync();
            var textureManagementSettings = await _textureManagementSettingsRepository.LoadAsync();
            var usageStatistics = await _usageStatisticsRepository.LoadAsync();

            var @event = new InitializeEvent(
                applicationSettings.Map<ApplicationSettings>(),
                games.Map<IEnumerable<Game>>(),
                gameProfileId?.Value,
                textureManagementSettings.Map<TextureManagementSettings>(),
                usageStatistics.Map<SharedModel.UsageStatistics>());

            foreach (var settings in games)
            {
                await _processWatcher.WatchAsync(settings.Id);
            }

            await context.PublishAsync(@event);
        }
    }
}