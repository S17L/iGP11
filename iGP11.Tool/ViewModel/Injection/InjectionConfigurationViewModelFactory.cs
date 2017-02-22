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
        private readonly IFindInjectionProfilesQuery _findInjectionProfilesQuery;
        private readonly IFindInjectionSettingsByIdQuery _findInjectionSettingsByIdQuery;
        private readonly IFindLastEditedInjectionSettingsQuery _findLastEditedInjectionSettingsQuery;
        private readonly INavigationService _navigationService;
        private readonly IPluginComponentFactory _pluginComponentFactory;
        private readonly ITaskRunner _runner;
        private readonly IEqualityComparer<ProxySettings> _stateEqualityComparer;

        public InjectionConfigurationViewModelFactory(
            DomainActionBuilder actionBuilder,
            ComponentViewModelFactory componentViewModelFactory,
            IFindInjectionProfilesQuery findInjectionProfilesQuery,
            IFindFirstLaunchTimeQuery findFirstLaunchTimeQuery,
            IFindInjectionSettingsByIdQuery findInjectionSettingsByIdQuery,
            IFindLastEditedInjectionSettingsQuery findLastEditedInjectionSettingsQuery,
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
            _findInjectionProfilesQuery = findInjectionProfilesQuery;
            _findFirstLaunchTimeQuery = findFirstLaunchTimeQuery;
            _findInjectionProfilesQuery = findInjectionProfilesQuery;
            _findInjectionSettingsByIdQuery = findInjectionSettingsByIdQuery;
            _findLastEditedInjectionSettingsQuery = findLastEditedInjectionSettingsQuery;
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
                _findInjectionProfilesQuery,
                _findFirstLaunchTimeQuery,
                _findInjectionSettingsByIdQuery,
                _findLastEditedInjectionSettingsQuery,
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