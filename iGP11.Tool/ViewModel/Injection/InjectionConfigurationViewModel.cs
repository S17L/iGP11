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
using iGP11.Tool.Shared.Model.GameSettings;
using iGP11.Tool.Shared.Notification;
using iGP11.Tool.Shared.Plugin;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public sealed class InjectionConfigurationViewModel : ViewModel,
                                                          IDisposable,
                                                          IEventHandler<ApplicationMinimizedEvent>,
                                                          IEventHandler<ApplicationActionEvent>,
                                                          IEventHandler<ApplicationRestoredEvent>,
                                                          IEventHandler<EditPluginComponentEvent>,
                                                          IEventHandler<PluginChangedEvent>,
                                                          IEventHandler<ReplacePluginComponentEvent>
    {
        private const int TaskInterval = 5000;

        private readonly DomainActionBuilder _actionBuilder;
        private readonly ComponentViewModelFactory _componentViewModelFactory;
        private readonly IDirectoryPicker _directoryPicker;
        private readonly IFilePicker _filePicker;
        private readonly IFindFirstLaunchTimeQuery _findFirstLaunchTimeQuery;
        private readonly IFindGamePackageByIdQuery _findGamePackageByIdQuery;
        private readonly IFindGamesQuery _findGamesQuery;
        private readonly IFindLastEditedGamePackageQuery _findLastEditedGamePackageQuery;
        private readonly ObservableRangeCollection<LookupViewModel> _gameProfiles = new ObservableRangeCollection<LookupViewModel>();
        private readonly ObservableRangeCollection<LookupViewModel> _games = new ObservableRangeCollection<LookupViewModel>();
        private readonly INavigationService _navigationService;
        private readonly IPluginFactory _pluginFactory;
        private readonly IProcessable _processable;
        private readonly IEventPublisher _publisher;
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly ITaskRunner _runner;
        private readonly IEqualityComparer<ProxySettings> _stateEqualityComparer;
        private bool _applicationActive = true;
        private IEnumerable<Game> _gamePackages;
        private IInjectionViewModel _injectionViewModel = new CollectDataViewModel();
        private ModeType _mode = ModeType.Injector;
        private GamePackage _package;
        private IComponentViewModel _pluginComponent;

        private ProxySettings _proxySettings;
        private IScheduler _scheduler;
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
            IEventPublisher publisher,
            IFilePicker filePicker,
            IPluginFactory pluginFactory,
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
            _publisher = publisher;
            _filePicker = filePicker;
            _pluginFactory = pluginFactory;
            _navigationService = navigationService;
            _processable = processable;
            _runner = runner;

            _publisher.Register<ApplicationActionEvent>(this);
            _publisher.Register<ApplicationMinimizedEvent>(this);
            _publisher.Register<ApplicationRestoredEvent>(this);

            ActionCommand = new ActionCommand(() => _queue.QueueTask(ExecuteActionAsync), IsActionEnabled);
            ActivationState = new StateViewModel(_runner);
            AddGameProfileCommand = new ActionCommand(() => _queue.QueueTask(AddGameProfileAsync), () => true);
            ChangedCommand = new ActionCommand(async () => await UpdateGameProfileAsync(), () => true);
            ClearGameFilePathCommand = new ActionCommand(async () => await ClearApplicationFilePathAsync(), () => !GameFilePath.IsNullOrEmpty());
            EditGameProfileCommand = new ActionCommand(() => _queue.QueueAction(EditPluginMainSettings), () => true);
            MoveToGameFilePathCommand = new ActionCommand(MoveToApplicationPath, () => !GameFilePath.IsNullOrEmpty());
            MoveToConfigurationDirectoryPathCommand = new ActionCommand(MoveToConfigurationDirectoryPath, () => !GameFilePath.IsNullOrEmpty());
            MoveToLogsDirectoryPathCommand = new ActionCommand(MoveToLogsDirectoryPath, () => !GameFilePath.IsNullOrEmpty());
            SwitchModeCommand = new ActionCommand(async () => await SwapModeAsync(), () => true);
            PickGameFilePathCommand = new ActionCommand(async () => await PickApplicationFilePathAsync(), () => true);
            PickPluginSettingsEditViewCommand = new GenericActionCommand<IComponentViewModel>(EditPluginComponent, () => true);
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

        public IActionCommand ChangedCommand { get; }

        public IActionCommand ClearGameFilePathCommand { get; }

        public IActionCommand EditGameProfileCommand { get; }

        public string FormattedLogsDirectoryPath => _injectionViewModel.FormattedLogsDirectoryPath;

        public string FormattedProxyDirectoryPath => _injectionViewModel.FormattedConfigurationDirectoryPath;

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

        public Guid GameId
        {
            get { return _package?.Game.Id ?? Guid.Empty; }
            set
            {
                if ((_package == null) || (_package.Game.Id == value))
                {
                    return;
                }

                _queue.QueueTask(() => ChangeGameAsync(value));
            }
        }

        public string GameName => _injectionViewModel?.GameName ?? string.Empty;

        public Guid GameProfileId
        {
            get { return _package?.GameProfile.Id ?? Guid.Empty; }
            set
            {
                if ((_package == null) || (_package.GameProfile.Id == value))
                {
                    return;
                }

                _queue.QueueTask(() => ChangeGameProfileAsync(value));
            }
        }

        public string GameProfileName => _injectionViewModel?.GameProfileName ?? string.Empty;

        public IEnumerable<LookupViewModel> GameProfiles => _gameProfiles;

        public IEnumerable<LookupViewModel> Games => _games;

        public bool HasGameFilePath => _injectionViewModel.GameFilePath.IsNotNullOrEmpty();

        public bool IsStandardMode => _mode == ModeType.Injector;

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

        public IActionCommand MoveToConfigurationDirectoryPathCommand { get; }

        public IActionCommand MoveToGameFilePathCommand { get; }

        public IActionCommand MoveToLogsDirectoryPathCommand { get; }

        public IActionCommand PickGameFilePathCommand { get; }

        public IActionCommand PickPluginSettingsEditViewCommand { get; }

        public IComponentViewModel PluginComponent
        {
            get { return _pluginComponent; }

            set
            {
                _pluginComponent = value;

                PluginViewModel.Select(value);
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

        public PluginViewModel PluginViewModel { get; private set; }

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
            _publisher.Unregister<ApplicationActionEvent>(this);
            _publisher.Unregister<ApplicationMinimizedEvent>(this);
            _publisher.Unregister<ApplicationRestoredEvent>(this);
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

        async Task IEventHandler<EditPluginComponentEvent>.HandleAsync(EditPluginComponentEvent @event)
        {
            _runner.Run(() => PluginComponent = @event.Component);

            await Task.Yield();
        }

        async Task IEventHandler<PluginChangedEvent>.HandleAsync(PluginChangedEvent @event)
        {
            _runner.Run(async () => await UpdateGameProfileAsync());

            await Task.Yield();
        }

        async Task IEventHandler<ReplacePluginComponentEvent>.HandleAsync(ReplacePluginComponentEvent @event)
        {
            _runner.Run(() =>
            {
                if (PluginComponent == @event.OldComponent)
                {
                    PluginComponent = @event.NewComponent;
                }
            });

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
                await _actionBuilder.Dispatch(new AddGameProfileCommand(addedProfile.ProfileName, gameId, addedProfile.BasedOnGameProfileId))
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
                .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await _publisher.PublishAsync(new ShutdownEvent()))
                .OnTimeout(async () => await _publisher.PublishAsync(new ShutdownEvent()))
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
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "GameProfileChanged", GameName, GameProfileName))
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
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "GameProfileChanged", GameName, GameProfileName))
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

        private void EditPluginComponent(IComponentViewModel viewModel)
        {
            PluginComponent = viewModel;
        }

        private void EditPluginMainSettings()
        {
            PluginComponent = null;
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

                await PublishUpdateStatusEventAsync(StatusType.Ok, "CommunicationEstablished");

                _status = null;
                _proxySettings = proxySettings;

                PluginViewModel = new PluginViewModel(
                    _pluginFactory.Create(proxySettings),
                    _publisher,
                    _navigationService,
                    _componentViewModelFactory);

                _injectionViewModel.GameName = proxySettings.GameName;
                _injectionViewModel.GameProfileName = proxySettings.GameProfileName;

                GameFilePath = proxySettings.GameFilePath;
                ProxyDirectoryPath = proxySettings.ProxyDirectoryPath;
                LogsDirectoryPath = proxySettings.LogsDirectoryPath;
                PluginType = proxySettings.PluginType;

                EditPluginMainSettings();
                EstimateCommunicatorActivationState(proxySettings.ActivationStatus);
                Validate();
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
                    await _publisher.PublishAsync(new ShowApplicationEvent());
                    break;
                case ApplicationAction.Injection:
                    await _publisher.PublishAsync(new ShowApplicationEvent());
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

            PluginViewModel = new PluginViewModel(
                _pluginFactory.Create(_package),
                _publisher,
                _navigationService,
                _componentViewModelFactory);

            await EstimateInjectorStateAsync();
            Validate();
            EditPluginMainSettings();
            StartScheduler();
        }

        private async Task InjectPluginAsync(bool hideApplication = false)
        {
            if (!IsInjectionEnabled())
            {
                await _publisher.PublishAsync(new ShowApplicationEvent());
                return;
            }

            using (new ProcessingScope(_processable))
            using (new DisabledSchedulerScope(_scheduler))
            {
                await PublishUpdateStatusEventAsync(StatusType.Information, "GameLaunching");

                var status = GameLaunchingStatus.Failed;
                await _actionBuilder.Dispatch(new StarGameCommand(_package.Game.Id))
                    .CompleteFor<ApplicationStartedNotification>((context, @event) => status = @event.Status)
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishErrorEventAsync(@event.Error))
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                switch (status)
                {
                    case GameLaunchingStatus.Completed:
                        await PublishUpdateStatusEventAsync(StatusType.Ok, "GameLaunchingStatusCompleted");
                        if (hideApplication)
                        {
                            await _publisher.PublishAsync(new HideApplicationToTrayEvent());
                        }

                        break;
                    case GameLaunchingStatus.PluginAlreadyLoaded:
                        await PublishUpdateStatusEventAsync(StatusType.Information, "GameLaunchingStatusPluginAlreadyLoaded");
                        break;
                    case GameLaunchingStatus.Failed:
                        await PublishUpdateStatusEventAsync(StatusType.Failed, "GameLaunchingStatusFailed");
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
            return (_mode == ModeType.Injector) && (_status == ActivationStatus.NotRunning) && (PluginViewModel?.IsValid == true);
        }

        private bool IsUpdateConfigurationEnabled()
        {
            return (_mode == ModeType.Communicator) && (PluginViewModel?.IsValid == true);
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

        private async Task PublishErrorEventAsync(Localizable error)
        {
            await _publisher.PublishAsync(new UpdateStatusEvent(Target.EntryPoint, StatusType.Failed, error.Localize()));
        }

        private async Task PublishGameProfileUpdatedEventAsync()
        {
            await PublishUpdateStatusEventAsync(StatusType.Ok, "GameProfileUpdated", DateTime.Now);
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
            await _publisher.PublishAsync(
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
            OnPropertyChanged(() => IsStandardMode);
            OnPropertyChanged(() => LogsDirectoryPath);
            OnPropertyChanged(() => PluginComponent);
            OnPropertyChanged(() => PluginType);
            OnPropertyChanged(() => PluginViewModel);
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

        private void RebindGameProfiles()
        {
            _gameProfiles.Clear();
            if (_package != null)
            {
                _gameProfiles.AddRange(_package.Game.Profiles.Select(profile => new LookupViewModel(profile.Id, profile.Name)));
            }
        }

        private void RebindGames()
        {
            _games.Clear();
            _games.AddRange(_gamePackages.Select(game => new LookupViewModel(game.Id, game.Name)));
        }

        private async Task RemoveGameProfileAsync()
        {
            var id = _package.GameProfile.Id;
            if (!_navigationService.ShowConfirmationDialog(
                    Target.EntryPoint,
                    Localization.Localization.Current.Get("RemoveGameProfileDialogTitle"),
                    Localization.Localization.Current.Get("RemoveGameProfileQuestion")))
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
                    .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "GameProfileRenamed"))
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                _package = await _findGamePackageByIdQuery.FindByGameProfileIdAsync(gameProfileId);

                RebindGameProfiles();
                OnPropertyChanged(() => GameProfileId);
                OnPropertyChanged(() => GameProfileName);
            }
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

                if ((_mode == ModeType.Communicator) || (PluginViewModel?.IsValid == false))
                {
                    return;
                }

                game = _package.Game.Clone();
            }

            await _actionBuilder.Dispatch(new UpdateGameCommand(game.Id, game.Name, game.FilePath))
                .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishGameProfileUpdatedEventAsync())
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

                if ((_mode == ModeType.Communicator) || (PluginViewModel?.IsValid == false))
                {
                    return;
                }

                gameProfile = _package.GameProfile.Clone();
            }

            await _actionBuilder.Dispatch(new UpdateGameProfileCommand(gameProfile))
                .CompleteFor<ActionSucceededNotification>(async (context, @event) => await PublishGameProfileUpdatedEventAsync())
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
                    .CompleteFor<ProxySettingsLoadedNotification>(async (context, @event) => await PublishGameProfileUpdatedEventAsync())
                    .CompleteFor<ErrorOccuredNotification>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Failed, "GameProfileUpdatingFailed", DateTime.Now))
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();
            }
        }

        private void Validate()
        {
            PluginViewModel.Validate();
            Rebind();
        }
    }
}