using System.Collections.Generic;

using iGP11.Library;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Common;
using iGP11.Tool.Model;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Injection
{
    public class InjectionConfigurationViewModelFactory
    {
        private readonly DomainActionBuilder _actionBuilder;
        private readonly ComponentViewModelFactory _componentViewModelFactory;
        private readonly IDirectoryPicker _directoryPicker;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFilePicker _filePicker;
        private readonly IFindFirstLaunchTimeQuery _findFirstLaunchTimeQuery;
        private readonly IFindGamePackageByIdQuery _findGamePackageByIdQuery;
        private readonly IFindGamesQuery _findGamesQuery;
        private readonly IFindLastEditedGamePackageQuery _findLastEditedGamePackageQuery;
        private readonly INavigationService _navigationService;
        private readonly IPluginComponentFactory _pluginComponentFactory;
        private readonly ITaskRunner _runner;
        private readonly IEqualityComparer<ProxySettings> _stateEqualityComparer;

        public InjectionConfigurationViewModelFactory(
            DomainActionBuilder actionBuilder,
            ComponentViewModelFactory componentViewModelFactory,
            IFindFirstLaunchTimeQuery findFirstLaunchTimeQuery,
            IFindGamePackageByIdQuery findGamePackageByIdQuery,
            IFindGamesQuery findGamesQuery,
            IFindLastEditedGamePackageQuery findLastEditedGamePackageQuery,
            IDirectoryPicker directoryPicker,
            IEqualityComparer<ProxySettings> stateEqualityComparer,
            IEventPublisher eventPublisher,
            IFilePicker filePicker,
            IPluginComponentFactory pluginComponentFactory,
            INavigationService navigationService,
            ITaskRunner runner)
        {
            _actionBuilder = actionBuilder;
            _componentViewModelFactory = componentViewModelFactory;
            _findFirstLaunchTimeQuery = findFirstLaunchTimeQuery;
            _findGamePackageByIdQuery = findGamePackageByIdQuery;
            _findGamesQuery = findGamesQuery;
            _findLastEditedGamePackageQuery = findLastEditedGamePackageQuery;
            _directoryPicker = directoryPicker;
            _stateEqualityComparer = stateEqualityComparer;
            _eventPublisher = eventPublisher;
            _filePicker = filePicker;
            _pluginComponentFactory = pluginComponentFactory;
            _navigationService = navigationService;
            _runner = runner;
        }

        public InjectionConfigurationViewModel Create(IProcessable processable)
        {
            return new InjectionConfigurationViewModel(
                _actionBuilder,
                _componentViewModelFactory,
                _findFirstLaunchTimeQuery,
                _findGamePackageByIdQuery,
                _findGamesQuery,
                _findLastEditedGamePackageQuery,
                _directoryPicker,
                _stateEqualityComparer,
                _eventPublisher,
                _filePicker,
                _pluginComponentFactory,
                _navigationService,
                processable,
                _runner);
        }
    }
}