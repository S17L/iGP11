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
using iGP11.Tool.ReadModel.Api.Model;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Notification;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public sealed class InjectionConfigurationViewModel : ViewModel,
                                                          IDisposable,
                                                          IEventHandler<ApplicationMinimizedEvent>,
                                                          IEventHandler<ApplicationActionEvent>,
                                                          IEventHandler<ApplicationRestoredEvent>,
                                                          IInjectionConfigurationViewModel
    {
        private const int TaskInterval = 5000;

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
        private readonly ObservableRangeCollection<IComponentViewModel> _plugin = new ObservableRangeCollection<IComponentViewModel>();
        private readonly IPluginComponentFactory _pluginComponentFactory;
        private readonly IProcessable _processable;
        private readonly ObservableRangeCollection<LookupViewModel> _games = new ObservableRangeCollection<LookupViewModel>();
        private readonly ObservableRangeCollection<LookupViewModel> _gameProfiles = new ObservableRangeCollection<LookupViewModel>();
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly ITaskRunner _runner;
        private readonly IEqualityComparer<ProxySettings> _stateEqualityComparer;
        private bool _applicationActive = true;
        private IInjectionViewModel _injectionViewModel = new CollectDataViewModel();
        private bool _isValid;
        private ModeType _mode = ModeType.Injector;
        private IComponentViewModel _pluginEditForm;

        private ProxySettings _proxySettings;
        private IScheduler _scheduler;
        private IEnumerable<Game> _gamePackages;
        private GamePackage _package;
        private ActivationStatus? _status;

        public InjectionConfigurationViewModel(
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
            IProcessable processable,
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
            _processable = processable;
            _runner = runner;

            _eventPublisher.Register<ApplicationActionEvent>(this);
            _eventPublisher.Register<ApplicationMinimizedEvent>(this);
            _eventPublisher.Register<ApplicationRestoredEvent>(this);

            ActionCommand = new ActionCommand(() => _queue.QueueTask(ExecuteActionAsync), IsActionEnabled);
            ActivationState = new StateViewModel(_runner);
            AddGameProfileCommand = new ActionCommand(() => _queue.QueueTask(AddGameProfileAsync), () => true);
            ChangedCommand = new ActionCommand(async () => await UpdateGameProfileAsync(), () => true);
            ClearGameFilePathCommand = new ActionCommand(async () => await ClearApplicationFilePathAsync(), () => !GameFilePath.IsNullOrEmpty());
            EditGameProfileCommand = new ActionCommand(() => _queue.QueueAction(ShowMainForm), () => true);
            MoveToGameFilePathCommand = new ActionCommand(MoveToApplicationPath, () => !GameFilePath.IsNullOrEmpty());
            MoveToConfigurationDirectoryPathCommand = new ActionCommand(MoveToConfigurationDirectoryPath, () => !GameFilePath.IsNullOrEmpty());
            MoveToLogsDirectoryPathCommand = new ActionCommand(MoveToLogsDirectoryPath, () => !GameFilePath.IsNullOrEmpty());
            SwitchModeCommand = new ActionCommand(async () => await SwapModeAsync(), () => true);
            PickGameFilePathCommand = new ActionCommand(async () => await PickApplicationFilePathAsync(), () => true);
            PickPluginSettingsEditViewCommand = new GenericActionCommand<IComponentViewModel>(PickPluginSettingsEditView, () => true);
            RemoveGameProfileCommand = new ActionCommand(async () => await RemoveGameProfileAsync(), () => _package?.Game.Profiles.Count > 1);
            RenameGameProfileCommand = new ActionCommand(async () => await RenameGameProfileAsync(), () => true);
            ValidationTriggeredCommand = new GenericActionCommand<ValidationResultEventArgs>(eventArgs => _queue.QueueAction(Validate), () => true);
        }

        ~InjectionConfigurationViewModel()
        {
            Dispose();
        }

        public IActionCommand ActionCommand { get; }

        public StateViewModel ActivationState { get; }

        public IActionCommand AddGameProfileCommand { get; }

        public string GameFilePath
        {
            get { return _injectionViewModel.GameFilePath; }
            set
            {
                _injectionViewModel.GameFilePath = value;

                OnPropertyChanged();
                OnPropertyChanged(() => FormattedProxyDirectoryPath);
            }
        }

        public IActionCommand ChangedCommand { get; }

        public IActionCommand ClearGameFilePathCommand { get; }

        public IActionCommand EditGameProfileCommand { get; }

        public string FormattedLogsDirectoryPath => _injectionViewModel.FormattedLogsDirectoryPath;

        public string FormattedProxyDirectoryPath => _injectionViewModel.FormattedConfigurationDirectoryPath;

        public bool HasGameFilePath => _injectionViewModel.GameFilePath.IsNotNullOrEmpty();

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

        public IActionCommand MoveToGameFilePathCommand { get; }

        public IActionCommand MoveToConfigurationDirectoryPathCommand { get; }

        public IActionCommand MoveToLogsDirectoryPathCommand { get; }

        public IActionCommand PickGameFilePathCommand { get; }

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

        public Guid GameId
        {
            get { return _package?.Game.Id ?? Guid.Empty; }
            set
            {
                if (_package == null || _package.Game.Id == value)
                {
                    return;
                }

                _queue.QueueTask(() => ChangeGameAsync(value));
            }
        }

        public Guid GameProfileId
        {
            get { return _package?.GameProfile.Id ?? Guid.Empty; }
            set
            {
                if (_package == null || _package.GameProfile.Id == value)
                {
                    return;
                }

                _queue.QueueTask(() => ChangeGameProfileAsync(value));
            }
        }

        public string GameName => _package?.Game.Name ?? string.Empty;

        public string GameProfileName => _package?.GameProfile.Name ?? string.Empty;

        public IEnumerable<LookupViewModel> Games => _games;

        public IEnumerable<LookupViewModel> GameProfiles => _gameProfiles;

        public string ProxyDirectoryPath
        {
            get { return _injectionViewModel.ProxyDirectoryPath; }
            set
            {
                _injectionViewModel.ProxyDirectoryPath = value;

                OnPropertyChanged();
                OnPropertyChanged(() => FormattedProxyDirectoryPath);
            }
        }

        public IActionCommand RemoveGameProfileCommand { get; }

        public IActionCommand RenameGameProfileCommand { get; }

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
                .CompleteFor<ActionSucceededNotification>()
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
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
            _runner.Run(async () => await EstimateState());

            await Task.Yield();
        }

        private async Task AddGameProfileAsync()
        {
            var gameId = _package.Game.Id;
            var addedProfile = _navigationService.ShowAddProfileDialog(Target.EntryPoint, _gameProfiles);
            if (addedProfile == null)
            {
                return;
            }

            using (new ProcessingScope(_processable))
            {
                var id = Guid.Empty;
                await _actionBuilder.Dispatch(new AddGameProfileCommand(addedProfile.ProfileName, gameId, addedProfile.BasedOnProfileId))
                    .CompleteFor<GameProfileAddedNotification>((context, @event) => id = @event.Id)
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                if (id != Guid.Empty)
                {
                    _package = await _findGamePackageByIdQuery.FindByGameIdAsync(gameId);
                    RebindGameProfiles();
                    Rebind();
                }
            }
        }

        private async Task BootstrapAsync(ModeType mode, ApplicationAction? action = null)
        {
            var initialized = false;
            await _actionBuilder.Dispatch(new InitializeCommand())
                .CompleteFor<ActionSucceededNotification>((context, @event) => initialized = true)
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await _eventPublisher.PublishAsync(new ShutdownEvent()))
                .OnTimeout(async () => await _eventPublisher.PublishAsync(new ShutdownEvent()))
                .Execute();

            if (initialized)
            {
                await SwitchModeAsync(mode, action);
            }
        }

        private async Task ChangeGameAsync(AggregateId gameId)
        {
            using (new ProcessingScope(_processable))
            {
                _package = await _findGamePackageByIdQuery.FindByGameIdAsync(gameId);

                await InitializeInjectorModeAsync();

                RebindGameProfiles();
                Rebind();

                await _actionBuilder.Dispatch(new UpdateLastEditedGameProfileCommand(_package.GameProfile.Id))
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileChanged", GameName, GameProfileName))
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }
        }

        private async Task ChangeGameProfileAsync(AggregateId gameProfileId)
        {
            using (new ProcessingScope(_processable))
            {
                _package = await _findGamePackageByIdQuery.FindByGameProfileIdAsync(gameProfileId);

                await InitializeInjectorModeAsync();

                Rebind();

                await _actionBuilder.Dispatch(new UpdateLastEditedGameProfileCommand(_package.GameProfile.Id))
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileChanged", GameName, GameProfileName))
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }
        }

        private async Task ClearApplicationFilePathAsync()
        {
            GameFilePath = string.Empty;
            await UpdateGameAsync();
        }

        private void EstimateCommunicatorActivationState(ActivationStatus status)
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

        private async Task EstimateCommunicatorStateAsync(bool lockingEnabled = false)
        {
            ProxySettings proxySettings = null;
            await _actionBuilder.Dispatch(new LoadProxySettingsCommand())
                .CompleteFor<ProxySettingsLoadedNotification>((context, @event) => proxySettings = @event.ProxySettings)
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Failed, "CommunicationError"))
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();

            using (await GetBlockingScope(lockingEnabled))
            {
                if ((_mode == ModeType.Injector) || (proxySettings == null))
                {
                    await InitializeInjectorModeAsync();

                    RebindGames();
                    RebindGameProfiles();
                    Rebind();

                    return;
                }

                if (_stateEqualityComparer.Equals(_proxySettings, proxySettings))
                {
                    EstimateCommunicatorActivationState(proxySettings.ActivationStatus);
                    return;
                }

                _status = null;
                _proxySettings = proxySettings;
                _plugin.Clear();
                _plugin.AddRange(_componentViewModelFactory.CreateEditable(_pluginComponentFactory.Create(proxySettings)));

                GameFilePath = proxySettings.GameFilePath;
                ProxyDirectoryPath = proxySettings.ProxyDirectoryPath;
                LogsDirectoryPath = proxySettings.LogsDirectoryPath;
                PluginType = proxySettings.PluginType;

                ShowMainForm();
                EstimateCommunicatorActivationState(proxySettings.ActivationStatus);
                Rebind();
            }
        }

        private void EstimateInjectorActivationState(ActivationStatus status)
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

        private async Task EstimateInjectorStateAsync(bool lockingEnabled = false)
        {
            var status = ActivationStatus.NotRetrievable;
            var applicationFilePath = GameFilePath;

            if (applicationFilePath.IsNotNullOrEmpty())
            {
                await _actionBuilder.Dispatch(new LoadProxyActivationStatusCommand(applicationFilePath))
                    .CompleteFor<ProxyActivationStatusLoadedNotification>((context, @event) => status = @event.Status)
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }

            using (await GetBlockingScope(lockingEnabled))
            {
                if ((_mode == ModeType.Communicator) || (_status == status))
                {
                    return;
                }

                _status = status;
                _proxySettings = null;

                EstimateInjectorActivationState(status);
                Rebind();
            }
        }

        private async Task EstimateState()
        {
            switch (_mode)
            {
                case ModeType.Injector:
                    await EstimateInjectorStateAsync(true);
                    break;
                case ModeType.Communicator:
                    await EstimateCommunicatorStateAsync(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

            if (_package == null)
            {
                _package = await _findLastEditedGamePackageQuery.FindAsync();
                _gamePackages = await _findGamesQuery.FindAllAsync();
            }

            _injectionViewModel = new InjectionViewModel(this, _package);
            _scheduler = new BlockingScheduler(() => _runner.Run(async () => await EstimateInjectorStateAsync(true)), TaskInterval);

            _plugin.Clear();
            _plugin.AddRange(_componentViewModelFactory.CreateEditable(_pluginComponentFactory.Create(_package)));

            await EstimateInjectorStateAsync();
            Validate();
            ShowMainForm();
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
                await _actionBuilder.Dispatch(new StarGameCommand(_package.Game.Id))
                    .CompleteFor<ApplicationStartedNotification>((context, @event) => status = @event.Status)
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishErrorEventAsync(@event.Error))
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
            _filePicker.OpenDirectory(GameFilePath);
        }

        private void MoveToConfigurationDirectoryPath()
        {
            _directoryPicker.Open(FormattedProxyDirectoryPath);
        }

        private void MoveToLogsDirectoryPath()
        {
            _directoryPicker.Open(FormattedLogsDirectoryPath);
        }

        private async Task PickApplicationFilePathAsync()
        {
            var path = _filePicker.Pick(GameFilePath);
            if (path.IsNullOrEmpty())
            {
                return;
            }

            GameFilePath = path;
            await UpdateGameAsync();
        }

        private void PickPluginSettingsEditView(IComponentViewModel viewModel)
        {
            PluginEditForm = viewModel;
        }

        private async Task PublishErrorEventAsync(Localizable error)
        {
            await _eventPublisher.PublishAsync(new UpdateStatusEvent(Target.EntryPoint, StatusType.Failed, error.Localize()));
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

        private async Task PublishUpdateStatusEventAsync(StatusType type, string key, params object[] arguments)
        {
            await _eventPublisher.PublishAsync(
                new UpdateStatusEvent(
                    Target.EntryPoint,
                    type,
                    string.Format(Localization.Localization.Current.Get(key), arguments)));
        }

        private void Rebind()
        {
            OnPropertyChanged(() => FormattedLogsDirectoryPath);
            OnPropertyChanged(() => FormattedProxyDirectoryPath);
            OnPropertyChanged(() => GameFilePath);
            OnPropertyChanged(() => GameName);
            OnPropertyChanged(() => GameProfileName);

            OnPropertyChanged(() => Games);
            OnPropertyChanged(() => GameProfiles);
            OnPropertyChanged(() => GameId);
            OnPropertyChanged(() => GameProfileId);

            OnPropertyChanged(() => HasGameFilePath);
            OnPropertyChanged(() => HasEditableSettings);
            OnPropertyChanged(() => IsStandardMode);
            OnPropertyChanged(() => IsValid);
            OnPropertyChanged(() => LogsDirectoryPath);
            OnPropertyChanged(() => Plugin);
            OnPropertyChanged(() => PluginEditForm);
            OnPropertyChanged(() => PluginType);
            OnPropertyChanged(() => ProxyDirectoryPath);

            ActionCommand.Rebind();
            AddGameProfileCommand.Rebind();
            ChangedCommand.Rebind();
            ClearGameFilePathCommand.Rebind();
            EditGameProfileCommand.Rebind();
            MoveToGameFilePathCommand.Rebind();
            MoveToConfigurationDirectoryPathCommand.Rebind();
            MoveToLogsDirectoryPathCommand.Rebind();
            PickGameFilePathCommand.Rebind();
            PickPluginSettingsEditViewCommand.Rebind();
            RemoveGameProfileCommand.Rebind();
            RenameGameProfileCommand.Rebind();
            SwitchModeCommand.Rebind();
            ValidationTriggeredCommand.Rebind();
        }

        private void RebindGames()
        {
            _games.Clear();
            _games.AddRange(_gamePackages.Select(game => new LookupViewModel(game.Id, game.Name)));
        }

        private void RebindGameProfiles()
        {
            _gameProfiles.Clear();
            if (_package != null)
            {
                _gameProfiles.AddRange(_package.Game.Profiles.Select(profile => new LookupViewModel(profile.Id, profile.Name)));
            }
        }

        private async Task RemoveGameProfileAsync()
        {
            var id = _package.GameProfile.Id;
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
                await _actionBuilder.Dispatch(new RemoveGameProfileCommand(id))
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileRemoved"))
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                _package = await _findLastEditedGamePackageQuery.FindAsync();

                RebindGameProfiles();
                Rebind();
            }
        }

        private async Task RenameGameProfileAsync()
        {
            var name = _navigationService.ShowRenameProfileDialog(Target.EntryPoint, _package.GameProfile.Name);
            if (name.IsNullOrEmpty())
            {
                return;
            }

            using (await _queue.GetBlockingScope())
            using (new ProcessingScope(_processable))
            {
                var gameProfileId = _package.GameProfile.Id;
                var gameProfile = _package.GameProfile.Clone();
                gameProfile.Name = name;

                await _actionBuilder.Dispatch(new UpdateGameProfileCommand(gameProfile))
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "ProfileRenamed"))
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                _package = await _findGamePackageByIdQuery.FindByGameProfileIdAsync(gameProfileId);

                RebindGameProfiles();
                OnPropertyChanged(() => GameProfileId);
                OnPropertyChanged(() => GameProfileName);
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

            using (await _queue.GetBlockingScope())
            {
                await SwitchModeAsync(mode);
            }
        }

        private async Task SwitchModeAsync(ModeType mode, ApplicationAction? action = null)
        {
            using (new ProcessingScope(_processable))
            {
                switch (mode)
                {
                    case ModeType.Injector:
                        await InitializeInjectorModeAsync();
                        RebindGames();
                        RebindGameProfiles();
                        Rebind();
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

        private async Task UpdateGameAsync()
        {
            Game game;
            using (await _queue.GetBlockingScope())
            {
                Validate();
                Rebind();

                if ((_mode == ModeType.Communicator) || !_isValid)
                {
                    return;
                }

                game = _package.Game.Clone();
            }

            await _actionBuilder.Dispatch(new UpdateGameCommand(game.Id, game.Name, game.FilePath))
                .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishProcessConfigurationUpdatedEventAsync())
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();
        }

        private async Task UpdateGameProfileAsync()
        {
            GameProfile gameProfile;
            using (await _queue.GetBlockingScope())
            {
                Validate();
                Rebind();

                if ((_mode == ModeType.Communicator) || !_isValid)
                {
                    return;
                }

                gameProfile = _package.GameProfile.Clone();
            }

            await _actionBuilder.Dispatch(new UpdateGameProfileCommand(gameProfile))
                .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishProcessConfigurationUpdatedEventAsync())
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();
        }

        private async Task UpdatePluginAsync()
        {
            using (new ProcessingScope(_processable))
            using (new DisabledSchedulerScope(_scheduler))
            {
                await _actionBuilder.Dispatch(new UpdateProxySettingsCommand(_proxySettings))
                    .CompleteFor<ProxySettingsLoadedNotification>(async (context, @event) => await PublishProcessConfigurationUpdatedEventAsync())
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Failed, "ProcessConfigurationUpdatingFailed", DateTime.Now))
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