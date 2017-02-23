using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Library.Scheduler;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Localization;
using iGP11.Tool.Model;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.InjectionSettings;
using iGP11.Tool.Shared.Notification;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public sealed class InjectionConfigurationViewModel : ViewModel,
                                                          IDisposable,
                                                          IEventHandler<ApplicationMinimizedEvent>,
                                                          IEventHandler<ApplicationRestoredEvent>,
                                                          IEventHandler<ApplicationActionEvent>,
                                                          IInjectionConfigurationViewModel
    {
        private const int SwitchModeDelay = 1000;
        private const int TaskInterval = 5000;

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
        private readonly ObservableRangeCollection<IComponentViewModel> _plugin = new ObservableRangeCollection<IComponentViewModel>();
        private readonly IPluginComponentFactory _pluginComponentFactory;
        private readonly IProcessable _processable;
        private readonly ObservableRangeCollection<ProfileViewModel> _profiles = new ObservableRangeCollection<ProfileViewModel>();
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly ITaskRunner _runner;
        private readonly IEqualityComparer<ProxySettings> _stateEqualityComparer;
        private bool _applicationActive = true;
        private IInjectionViewModel _injectionViewModel = new CollectDataViewModel();
        private bool _isValid;
        private ModeType _mode = ModeType.Injector;
        private IComponentViewModel _pluginEditForm;
        private Guid _profileId = Guid.Empty;

        private ProxySettings _proxySettings;
        private IScheduler _scheduler;
        private InjectionSettings _settings;
        private ActivationStatus? _status;

        public InjectionConfigurationViewModel(
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
            IProcessable processable,
            ITaskRunner runner)
        {
            _actionBuilder = actionBuilder;
            _componentViewModelFactory = componentViewModelFactory;
            _findInjectionProfilesQuery = findInjectionProfilesQuery;
            _findFirstLaunchTimeQuery = findFirstLaunchTimeQuery;
            _findInjectionSettingsByIdQuery = findInjectionSettingsByIdQuery;
            _findLastEditedInjectionSettingsQuery = findLastEditedInjectionSettingsQuery;
            _directoryPicker = directoryPicker;
            _stateEqualityComparer = stateEqualityComparer;
            _eventPublisher = eventPublisher;
            _filePicker = filePicker;
            _pluginComponentFactory = pluginComponentFactory;
            _navigationService = navigationService;
            _processable = processable;
            _runner = runner;

            _eventPublisher.Register<ApplicationActionEvent>(this);
            _eventPublisher.Register<ApplicationMinimizedEvent>(this);
            _eventPublisher.Register<ApplicationRestoredEvent>(this);

            ActionCommand = new ActionCommand(() => _queue.QueueTask(ExecuteActionAsync), IsActionEnabled);
            ActivationState = new StateViewModel(_runner);
            AddProfileCommand = new ActionCommand(() => _queue.QueueTask(AddProfileAsync), () => true);
            ChangedCommand = new ActionCommand(async () => await UpdateInjectionSettingsAsync(), () => true);
            ClearApplicationPathCommand = new ActionCommand(async () => await ClearApplicationFilePathAsync(), () => !ApplicationFilePath.IsNullOrEmpty());
            EditInjectionSettingsCommand = new ActionCommand(() => _queue.QueueAction(ShowMainForm), () => true);
            MoveToApplicationPathCommand = new ActionCommand(MoveToApplicationPath, () => !ApplicationFilePath.IsNullOrEmpty());
            MoveToConfigurationDirectoryPathCommand = new ActionCommand(MoveToConfigurationDirectoryPath, () => !ApplicationFilePath.IsNullOrEmpty());
            MoveToLogsDirectoryPathCommand = new ActionCommand(MoveToLogsDirectoryPath, () => !ApplicationFilePath.IsNullOrEmpty());
            SwitchModeCommand = new ActionCommand(() => _queue.QueueTask(SwapModeAsync), () => true);
            PickApplicationPathCommand = new ActionCommand(async () => await PickApplicationFilePathAsync(), () => true);
            PickPluginSettingsEditViewCommand = new GenericActionCommand<IComponentViewModel>(PickPluginSettingsEditView, () => true);
            RemoveProfileCommand = new ActionCommand(async () => await RemoveProfileAsync(), () => _profiles.Count > 1);
            RenameProfileCommand = new ActionCommand(async () => await RenameProfileAsync(), () => true);
            ValidationTriggeredCommand = new GenericActionCommand<ValidationResultEventArgs>(eventArgs => _queue.QueueAction(Validate), () => true);
        }

        ~InjectionConfigurationViewModel()
        {
            Dispose();
        }

        public IActionCommand ActionCommand { get; }

        public StateViewModel ActivationState { get; }

        public IActionCommand AddProfileCommand { get; }

        public string ApplicationFilePath
        {
            get { return _injectionViewModel.ApplicationFilePath; }
            set
            {
                _injectionViewModel.ApplicationFilePath = value;

                OnPropertyChanged();
                OnPropertyChanged(() => FormattedConfigurationDirectoryPath);
            }
        }

        public IActionCommand ChangedCommand { get; }

        public IActionCommand ClearApplicationPathCommand { get; }

        public string ConfigurationDirectoryPath
        {
            get { return _injectionViewModel.ConfigurationDirectoryPath; }
            set
            {
                _injectionViewModel.ConfigurationDirectoryPath = value;

                OnPropertyChanged();
                OnPropertyChanged(() => FormattedConfigurationDirectoryPath);
            }
        }

        public IActionCommand EditInjectionSettingsCommand { get; }

        public bool EstablishCommunication
        {
            get { return (_settings != null) && _settings.EstablishCommunication; }
            set
            {
                if (_settings == null)
                {
                    return;
                }

                _settings.EstablishCommunication = value;
                QueueUpdateInjectionSettings();
            }
        }

        public string FormattedConfigurationDirectoryPath => _injectionViewModel.FormattedConfigurationDirectoryPath;

        public string FormattedLogsDirectoryPath => _injectionViewModel.FormattedLogsDirectoryPath;

        public bool HasApplicationFilePath => _injectionViewModel.ApplicationFilePath.IsNotNullOrEmpty();

        public bool HasEditableSettings => !_plugin.IsNullOrEmpty();

        public bool IsStandardMode => _mode == ModeType.Injector;

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid == value)
                {
                    return;
                }

                _isValid = value;

                Rebind();
                OnPropertyChanged();
            }
        }

        public string LogsDirectoryPath
        {
            get { return _injectionViewModel.LogsDirectoryPath; }
            set
            {
                _injectionViewModel.LogsDirectoryPath = value;

                OnPropertyChanged();
                OnPropertyChanged(() => FormattedLogsDirectoryPath);
            }
        }

        public IActionCommand MoveToApplicationPathCommand { get; }

        public IActionCommand MoveToConfigurationDirectoryPathCommand { get; }

        public IActionCommand MoveToLogsDirectoryPathCommand { get; }

        public IActionCommand PickApplicationPathCommand { get; }

        public IActionCommand PickPluginSettingsEditViewCommand { get; }

        public IEnumerable<IComponentViewModel> Plugin => _plugin;

        public IComponentViewModel PluginEditForm
        {
            get { return _pluginEditForm; }

            set
            {
                _pluginEditForm = value;
                OnPropertyChanged();
            }
        }

        public PluginType PluginType
        {
            get { return _injectionViewModel.PluginType; }
            set
            {
                _injectionViewModel.PluginType = value;
                OnPropertyChanged();
            }
        }

        public Guid ProfileId
        {
            get { return _profileId; }
            set
            {
                if (_profileId == value)
                {
                    return;
                }

                _queue.QueueTask(() => ChangeProfileAsync(value));
            }
        }

        public string ProfileName => _profiles.Any(profile => profile.Id == _profileId)
                                         ? _profiles.First(profile => profile.Id == _profileId).Name
                                         : string.Empty;

        public IEnumerable<ProfileViewModel> Profiles => _profiles;

        public IActionCommand RemoveProfileCommand { get; }

        public IActionCommand RenameProfileCommand { get; }

        public IActionCommand SwitchModeCommand { get; }

        public IActionCommand ValidationTriggeredCommand { get; }

        public async Task BootstrapAsync(ApplicationAction? action = null)
        {
            await BootstrapAsync(ModeType.Injector, action);
        }

        public void Dispose()
        {
            _eventPublisher.Unregister<ApplicationActionEvent>(this);
            _eventPublisher.Unregister<ApplicationMinimizedEvent>(this);
            _eventPublisher.Unregister<ApplicationRestoredEvent>(this);
        }

        public void RebindPlugin()
        {
            foreach (var viewModel in _plugin)
            {
                viewModel.Rebind();
            }
        }

        public async Task ShowReleaseNotesAsync()
        {
            if ((await _findFirstLaunchTimeQuery.IsFirstRunAsync()).HasValue)
            {
                return;
            }

            _navigationService.ShowInformationDialog(
                Target.EntryPoint,
                $"{Localization.Localization.Current.Get("ReleaseNotesTitle")} {Assembly.GetEntryAssembly().GetAssemblyInformation().DisplayVersion}",
                Localization.Localization.Current.Get("ReleaseNotesDescription"));

            await _actionBuilder.Dispatch(new IndicateFirstLaunchCommand(DateTime.Now))
                .CompleteFor<ActionSucceededEvent>()
                .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();
        }

        async Task IEventHandler<ApplicationActionEvent>.HandleAsync(ApplicationActionEvent @event)
        {
            _runner.Run(() => _queue.QueueTask(() => HandleActionAsync(@event.Action)));
            await Task.Yield();
        }

        async Task IEventHandler<ApplicationMinimizedEvent>.HandleAsync(ApplicationMinimizedEvent @event)
        {
            _applicationActive = false;
            _scheduler?.Stop();

            await Task.Yield();
        }

        async Task IEventHandler<ApplicationRestoredEvent>.HandleAsync(ApplicationRestoredEvent @event)
        {
            _applicationActive = true;
            _scheduler?.Start();

            await Task.Yield();
        }

        private async Task AddProfileAsync()
        {
            var addedProfile = _navigationService.ShowAddProfileDialog(Target.EntryPoint, _profiles);
            if (addedProfile == null)
            {
                return;
            }

            using (new ProcessingScope(_processable))
            {
                var id = Guid.Empty;
                await _actionBuilder.Dispatch(new AddInjectionSettingsCommand(addedProfile.BasedOnProfileId, addedProfile.ProfileName))
                    .CompleteFor<InjectionSettingsAddedEvent>((context, @event) => id = @event.Id)
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                if (id == Guid.Empty)
                {
                    return;
                }

                await RebindProfilesAsync();
                ProfileId = id;
            }
        }

        private async Task BootstrapAsync(ModeType mode, ApplicationAction? action = null)
        {
            var initialized = false;
            await _actionBuilder.Dispatch(new InitializeCommand())
                .CompleteFor<ActionSucceededEvent>((context, @event) => initialized = true)
                .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await _eventPublisher.PublishAsync(new ShutdownEvent()))
                .OnTimeout(async () => await _eventPublisher.PublishAsync(new ShutdownEvent()))
                .Execute();

            if (initialized)
            {
                await RebindProfilesAsync();
                await SwitchModeAsync(mode, action);
            }
        }

        private async Task ChangeProfileAsync(AggregateId profileId)
        {
            using (new ProcessingScope(_processable))
            {
                _profileId = profileId;

                await InitializeInjectorModeAsync();
                await _actionBuilder.Dispatch(new UpdateLastEditedInjectionSettingsCommand(_profileId))
                    .CompleteFor<ActionSucceededEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileChanged", ProfileName))
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }
        }

        private async Task ClearApplicationFilePathAsync()
        {
            ApplicationFilePath = string.Empty;
            await UpdateInjectionSettingsAsync();
        }

        private async Task EstimateCommunicatorStateAsync(bool lockingEnabled = false)
        {
            ProxySettings proxySettings = null;
            await _actionBuilder.Dispatch(new LoadProxySettingsCommand())
                .CompleteFor<ProxySettingsLoadedEvent>((context, @event) => proxySettings = @event.ProxySettings)
                .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Failed, "CommunicationError"))
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();

            using (await GetBlockingScope(lockingEnabled))
            {
                if (proxySettings == null)
                {
                    await InitializeInjectorModeAsync();
                    return;
                }

                if (_stateEqualityComparer.Equals(_proxySettings, proxySettings))
                {
                    EvaluateCommunicatorActivationState(proxySettings.ActivationStatus);
                    return;
                }

                _status = null;
                _proxySettings = proxySettings;
                _plugin.Clear();
                _plugin.AddRange(_componentViewModelFactory.CreateEditable(_pluginComponentFactory.Create(proxySettings)));

                ApplicationFilePath = proxySettings.ApplicationFilePath;
                ConfigurationDirectoryPath = proxySettings.ConfigurationDirectoryPath;
                LogsDirectoryPath = proxySettings.LogsDirectoryPath;
                PluginType = proxySettings.PluginType;

                ShowMainForm();
                EvaluateCommunicatorActivationState(proxySettings.ActivationStatus);
                Rebind();
            }
        }

        private async Task EstimateInjectorStateAsync(bool lockingEnabled = false)
        {
            var status = ActivationStatus.NotRetrievable;
            var applicationFilePath = ApplicationFilePath;

            if (applicationFilePath.IsNotNullOrEmpty())
            {
                await _actionBuilder.Dispatch(new LoadProxyActivationStatusCommand(applicationFilePath))
                    .CompleteFor<ProxyActivationStatusLoadedEvent>((context, @event) => status = @event.Status)
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }

            using (await GetBlockingScope(lockingEnabled))
            {
                if (_status == status)
                {
                    return;
                }

                _status = status;
                _proxySettings = null;

                EvaluateInjectorActivationState(status);
                Rebind();
            }
        }

        private void EvaluateCommunicatorActivationState(ActivationStatus status)
        {
            switch (status)
            {
                case ActivationStatus.NotRetrievable:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusNotRetrievable"));
                    break;
                case ActivationStatus.NotRunning:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusNotRunning"));
                    break;
                case ActivationStatus.Running:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusRunning"));
                    break;
                case ActivationStatus.PluginLoaded:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusPluginLoaded"));
                    break;
                case ActivationStatus.PluginActivationPending:
                    ActivationState.Set(StatusType.Information, Localization.Localization.Current.Get("ActivationStatusPluginActivationPending"));
                    break;
                case ActivationStatus.PluginActivated:
                    ActivationState.Set(StatusType.Ok, Localization.Localization.Current.Get("ActivationStatusPluginActivated"));
                    break;
                case ActivationStatus.PluginActivationFailed:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusPluginActivationFailed"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void EvaluateInjectorActivationState(ActivationStatus status)
        {
            switch (status)
            {
                case ActivationStatus.NotRetrievable:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusNotRetrievable"));
                    break;
                case ActivationStatus.NotRunning:
                    ActivationState.Set(StatusType.Failed, Localization.Localization.Current.Get("ActivationStatusNotRunning"));
                    break;
                case ActivationStatus.Running:
                    ActivationState.Set(StatusType.Information, Localization.Localization.Current.Get("ActivationStatusRunning"));
                    break;
                case ActivationStatus.PluginLoaded:
                    ActivationState.Set(StatusType.Ok, Localization.Localization.Current.Get("ActivationStatusPluginLoaded"));
                    break;
                case ActivationStatus.PluginActivationPending:
                case ActivationStatus.PluginActivated:
                case ActivationStatus.PluginActivationFailed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private async Task ExecuteActionAsync()
        {
            switch (_mode)
            {
                case ModeType.Injector:
                    await InjectPluginAsync();
                    break;
                case ModeType.Communicator:
                    await UpdatePluginAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<IDisposable> GetBlockingScope(bool lockingEnabled)
        {
            return lockingEnabled
                       ? await _queue.GetBlockingScope()
                       : Task.FromResult<IDisposable>(null);
        }

        private async Task HandleActionAsync(ApplicationAction applicationAction)
        {
            if (_processable.IsProcessing)
            {
                return;
            }

            switch (applicationAction)
            {
                case ApplicationAction.Default:
                    await _eventPublisher.PublishAsync(new ShowApplicationEvent());
                    break;
                case ApplicationAction.Injection:
                    await _eventPublisher.PublishAsync(new ShowApplicationEvent());
                    await InjectPluginAsync(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationAction), applicationAction, null);
            }
        }

        private async Task InitializeCommunicatorModeAsync()
        {
            _mode = ModeType.Communicator;
            _injectionViewModel = new CollectDataViewModel();
            _scheduler?.Dispose();
            _scheduler = new BlockingScheduler(() => _runner.Run(async () => await EstimateCommunicatorStateAsync(true)), TaskInterval);

            await EstimateCommunicatorStateAsync();
            StartScheduler();
        }

        private async Task InitializeInjectorModeAsync()
        {
            _mode = ModeType.Injector;
            _scheduler?.Dispose();

            if (_settings == null)
            {
                _settings = await _findLastEditedInjectionSettingsQuery.FindAsync();
                _profileId = _settings.Id;
            }
            else if (_settings.Id != _profileId)
            {
                _settings = await _findInjectionSettingsByIdQuery.FindByIdAsync(_profileId);
            }

            _injectionViewModel = new InjectionViewModel(this, _settings);
            _scheduler = new BlockingScheduler(() => _runner.Run(async () => await EstimateInjectorStateAsync(true)), TaskInterval);

            _plugin.Clear();
            _plugin.AddRange(_componentViewModelFactory.CreateEditable(_pluginComponentFactory.Create(_settings)));

            await EstimateInjectorStateAsync();
            Validate();
            ShowMainForm();
            Rebind();
            StartScheduler();
        }

        private async Task InjectPluginAsync(bool hideApplication = false)
        {
            if (!IsInjectionEnabled())
            {
                await _eventPublisher.PublishAsync(new ShowApplicationEvent());
                return;
            }

            using (new ProcessingScope(_processable))
            using (new DisabledSchedulerScope(_scheduler))
            {
                await PublishUpdateStatusEventAsync(StatusType.Information, "InjectionStarted");

                var status = InjectionStatus.Failed;
                await _actionBuilder.Dispatch(new StartApplicationCommand(_settings.Id))
                    .CompleteFor<ApplicationStartedEvent>((context, @event) => status = @event.Status)
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishErrorEventAsync(@event.Error))
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                switch (status)
                {
                    case InjectionStatus.Completed:
                        await PublishUpdateStatusEventAsync(StatusType.Ok, "InjectionCompleted");
                        if (hideApplication)
                        {
                            await _eventPublisher.PublishAsync(new HideApplicationToTrayEvent());
                        }

                        if (_settings.EstablishCommunication)
                        {
                            await Task.Delay(SwitchModeDelay);
                            await SwapModeAsync();
                        }

                        break;
                    case InjectionStatus.PluginAlreadyLoaded:
                        await PublishUpdateStatusEventAsync(StatusType.Information, "PluginAlreadyLoaded");
                        break;
                    case InjectionStatus.Failed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool IsActionEnabled()
        {
            return _mode == ModeType.Injector
                       ? IsInjectionEnabled()
                       : IsUpdateConfigurationEnabled();
        }

        private bool IsInjectionEnabled()
        {
            return (_mode == ModeType.Injector) && (_status == ActivationStatus.NotRunning) && _isValid;
        }

        private bool IsUpdateConfigurationEnabled()
        {
            return (_mode == ModeType.Communicator) && _isValid;
        }

        private void MoveToApplicationPath()
        {
            _filePicker.OpenDirectory(ApplicationFilePath);
        }

        private void MoveToConfigurationDirectoryPath()
        {
            _directoryPicker.Open(FormattedConfigurationDirectoryPath);
        }

        private void MoveToLogsDirectoryPath()
        {
            _directoryPicker.Open(FormattedLogsDirectoryPath);
        }

        private async Task PickApplicationFilePathAsync()
        {
            var path = _filePicker.Pick(ApplicationFilePath);
            if (path.IsNullOrEmpty())
            {
                return;
            }

            ApplicationFilePath = path;
            await UpdateInjectionSettingsAsync();
        }

        private void PickPluginSettingsEditView(IComponentViewModel viewModel)
        {
            PluginEditForm = viewModel;
        }

        private async Task PublishProcessConfigurationUpdatedEventAsync()
        {
            await PublishUpdateStatusEventAsync(StatusType.Ok, "ProcessConfigurationUpdated", DateTime.Now);
        }

        private async Task PublishTimeoutEventAsync()
        {
            await PublishUpdateStatusEventAsync(StatusType.Failed, "OperationTimeout");
        }

        private async Task PublishUnknownErrorEventAsync()
        {
            await PublishUpdateStatusEventAsync(StatusType.Failed, "UnknownError");
        }

        private async Task PublishErrorEventAsync(Localizable error)
        {
            await _eventPublisher.PublishAsync(new UpdateStatusEvent(Target.EntryPoint, StatusType.Failed, error.Localize()));
        }

        private async Task PublishUpdateStatusEventAsync(StatusType type, string key, params object[] arguments)
        {
            await _eventPublisher.PublishAsync(
                new UpdateStatusEvent(
                    Target.EntryPoint,
                    type,
                    string.Format(Localization.Localization.Current.Get(key), arguments)));
        }

        private async void QueueUpdateInjectionSettings()
        {
            await UpdateInjectionSettingsAsync();
        }

        private void Rebind()
        {
            OnPropertyChanged(() => ApplicationFilePath);
            OnPropertyChanged(() => ConfigurationDirectoryPath);
            OnPropertyChanged(() => EstablishCommunication);
            OnPropertyChanged(() => FormattedConfigurationDirectoryPath);
            OnPropertyChanged(() => FormattedLogsDirectoryPath);
            OnPropertyChanged(() => HasApplicationFilePath);
            OnPropertyChanged(() => HasEditableSettings);
            OnPropertyChanged(() => IsStandardMode);
            OnPropertyChanged(() => IsValid);
            OnPropertyChanged(() => LogsDirectoryPath);
            OnPropertyChanged(() => Plugin);
            OnPropertyChanged(() => PluginEditForm);
            OnPropertyChanged(() => PluginType);
            OnPropertyChanged(() => ProfileId);
            OnPropertyChanged(() => ProfileName);

            ActionCommand.Rebind();
            AddProfileCommand.Rebind();
            ChangedCommand.Rebind();
            ClearApplicationPathCommand.Rebind();
            EditInjectionSettingsCommand.Rebind();
            MoveToApplicationPathCommand.Rebind();
            MoveToConfigurationDirectoryPathCommand.Rebind();
            MoveToLogsDirectoryPathCommand.Rebind();
            PickApplicationPathCommand.Rebind();
            PickPluginSettingsEditViewCommand.Rebind();
            RemoveProfileCommand.Rebind();
            RenameProfileCommand.Rebind();
            SwitchModeCommand.Rebind();
            ValidationTriggeredCommand.Rebind();
        }

        private async Task RebindProfilesAsync()
        {
            _profiles.Clear();
            _profiles.AddRange((await _findInjectionProfilesQuery.FindAllAsync())
                .Select(profile => new ProfileViewModel(profile.Id, profile.Name)));

            OnPropertyChanged(() => ProfileId);
        }

        private async Task RemoveProfileAsync()
        {
            var id = _settings.Id;
            if (!_navigationService.ShowConfirmationDialog(
                    Target.EntryPoint,
                    Localization.Localization.Current.Get("RemoveProfileDialogTitle"),
                    Localization.Localization.Current.Get("RemoveProfileQuestion")))
            {
                return;
            }

            using (await _queue.GetBlockingScope())
            using (new ProcessingScope(_processable))
            {
                await _actionBuilder.Dispatch(new RemoveInjectionSettingsCommand(id))
                    .CompleteFor<ActionSucceededEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileRemoved"))
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                await RebindProfilesAsync();

                ProfileId = (_profileId == Guid.Empty) || (_profileId == id)
                                ? _profiles.First(profile => profile.Id != id).Id
                                : _profileId;

                OnPropertyChanged(() => ProfileId);
            }
        }

        private async Task RenameProfileAsync()
        {
            var name = _navigationService.ShowRenameProfileDialog(Target.EntryPoint, _settings.Name);
            if (name.IsNullOrEmpty())
            {
                return;
            }

            using (await _queue.GetBlockingScope())
            using (new ProcessingScope(_processable))
            {
                _settings.Name = name;
                await _actionBuilder.Dispatch(new UpdateInjectionSettingsCommand(_settings))
                    .CompleteFor<ActionSucceededEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileRenamed"))
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                await RebindProfilesAsync();
                OnPropertyChanged(() => ProfileName);
            }
        }

        private void ShowMainForm()
        {
            PluginEditForm = null;
        }

        private void StartScheduler()
        {
            if (_applicationActive)
            {
                _scheduler.Start();
            }
        }

        private async Task SwapModeAsync()
        {
            var mode = _mode == ModeType.Injector
                           ? ModeType.Communicator
                           : ModeType.Injector;

            await SwitchModeAsync(mode);
        }

        private async Task SwitchModeAsync(ModeType mode, ApplicationAction? action = null)
        {
            using (new ProcessingScope(_processable))
            {
                switch (mode)
                {
                    case ModeType.Injector:
                        await InitializeInjectorModeAsync();
                        break;
                    case ModeType.Communicator:
                        await InitializeCommunicatorModeAsync();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (action.HasValue)
            {
                await HandleActionAsync(action.Value);
            }
        }

        private async Task UpdateInjectionSettingsAsync()
        {
            InjectionSettings settings;
            using (await _queue.GetBlockingScope())
            {
                Validate();
                Rebind();

                if ((_mode == ModeType.Communicator) || !_isValid)
                {
                    return;
                }

                settings = _settings.Clone();
            }

            await _actionBuilder.Dispatch(new UpdateInjectionSettingsCommand(settings))
                .CompleteFor<ActionSucceededEvent>(async (context, @event) => await PublishProcessConfigurationUpdatedEventAsync())
                .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();
        }

        private async Task UpdatePluginAsync()
        {
            using (new ProcessingScope(_processable))
            using (new DisabledSchedulerScope(_scheduler))
            {
                await _actionBuilder.Dispatch(new UpdateProxySettingsCommand(_proxySettings))
                    .CompleteFor<ProxySettingsLoadedEvent>(async (context, @event) => await PublishProcessConfigurationUpdatedEventAsync())
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Failed, "ProcessConfigurationUpdatingFailed", DateTime.Now))
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }
        }

        private void Validate()
        {
            IsValid = _plugin.All(settings => !settings.HasErrors);
        }
    }
}