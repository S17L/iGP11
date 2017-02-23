using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Library.Scheduler;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model;
using iGP11.Tool.Shared.Model.TextureManagementSettings;
using iGP11.Tool.Shared.Notification;

using ApplicationModel = iGP11.Tool.Application.Api.Model;

namespace iGP11.Tool.ViewModel.Texture
{
    public sealed class TextureManagementWindowViewModel : ViewModel,
                                                           IDisposable,
                                                           IEventHandler<UpdateStatusEvent>,
                                                           IProcessable,
                                                           ITextureManagementViewModel
    {
        private readonly DomainActionBuilder _actionBuilder;
        private readonly IDirectoryPicker _directoryPicker;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFilePicker _filePicker;
        private readonly IFindTextureManagementSettingsQuery _findTextureManagementSettingsQuery;
        private readonly BlockingTaskQueue _queue = new BlockingTaskQueue();
        private readonly ObservableRangeCollection<TextureNodeViewModel> _textures = new ObservableRangeCollection<TextureNodeViewModel>();

        private bool? _isProcessing = false;
        private IAsynchronousScheduler _scheduler;
        private TextureManagementSettings _settings;

        public TextureManagementWindowViewModel(
            DomainActionBuilder actionBuilder,
            IDirectoryPicker directoryPicker,
            IEventPublisher eventPublisher,
            IFilePicker filePicker,
            IFindTextureManagementSettingsQuery findTextureManagementSettingsQuery,
            IImageProvider emptyImageProvider,
            ITaskRunner dispatcher)
        {
            _actionBuilder = actionBuilder;
            _directoryPicker = directoryPicker;
            _eventPublisher = eventPublisher;
            _filePicker = filePicker;
            _findTextureManagementSettingsQuery = findTextureManagementSettingsQuery;
            _eventPublisher.Register(this);

            StatusViewModel = new StatusViewModel(dispatcher);
            TexturePreview = new TexturePreviewViewModel(_actionBuilder, _eventPublisher, this, emptyImageProvider);

            AbortCommand = new ActionCommand(AbortConversion, IsAbortConversionEnabled);
            CheckAllCommand = new ActionCommand(CheckAll, IsCheckAllEnabled);
            ConvertMultiCommand = new ActionCommand(() => _queue.QueueTask(ConvertMultiAsync), IsMultiConversionEnabled);
            ConvertSingleCommand = new ActionCommand(() => _queue.QueueTask(ConvertSingleAsync), IsSingleConversionEnabled);
            MoveToDestinationDirectoryCommand = new ActionCommand(MoveToDestinationDirectory, () => !DestinationDirectory.IsNullOrEmpty());
            MoveToSourceDirectoryCommand = new ActionCommand(MoveToSourceDirectory, () => !SourceDirectory.IsNullOrEmpty());
            MultiChangedCommand = new ActionCommand(() => _queue.QueueTask(OnChangedAsync), () => true);
            PickDestinationDirectoryCommand = new ActionCommand(() => _queue.QueueTask(PickDestinationDirectoryAsync), () => true);
            PickSourceDirectoryCommand = new ActionCommand(() => _queue.QueueTask(PickSourceDirectoryAsync), () => true);
            SingleChangedCommand = new ActionCommand(() => _queue.QueueTask(OnSingleChangedAsync), () => true);
            UncheckAllCommand = new ActionCommand(UncheckAll, IsUncheckAllEnabled);
        }

        ~TextureManagementWindowViewModel()
        {
            Dispose();
        }

        public IActionCommand AbortCommand { get; }

        public IActionCommand CheckAllCommand { get; }

        public Srgb ColorSpace => _settings?.SingleConversion.ColorSpace ?? Srgb.None;

        public IActionCommand ConvertMultiCommand { get; }

        public IActionCommand ConvertSingleCommand { get; }

        public string DestinationDirectory
        {
            get { return _settings?.DestinationDirectory ?? string.Empty; }
            set
            {
                if ((_settings == null) || (_settings.DestinationDirectory == value))
                {
                    return;
                }

                _settings.DestinationDirectory = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get { return _isProcessing ?? false; }
            set
            {
                if (_isProcessing == value)
                {
                    return;
                }

                _isProcessing = value;

                OnPropertyChanged();
                Rebind();
            }
        }

        public IActionCommand MoveToDestinationDirectoryCommand { get; }

        public IActionCommand MoveToSourceDirectoryCommand { get; }

        public IActionCommand MultiChangedCommand { get; }

        public TextureConversionSettings MultiTextureConversionSettings => _settings?.MultiConversion;

        public IActionCommand PickDestinationDirectoryCommand { get; }

        public IActionCommand PickSourceDirectoryCommand { get; }

        public IActionCommand SingleChangedCommand { get; }

        public TextureConversionSettings SingleTextureConversionConfiguration => _settings?.SingleConversion;

        public string SourceDirectory
        {
            get { return _settings?.SourceDirectory ?? string.Empty; }
            set
            {
                if ((_settings == null) || (_settings.SourceDirectory == value))
                {
                    return;
                }

                _settings.SourceDirectory = value;
                OnPropertyChanged();
            }
        }

        public StatusViewModel StatusViewModel { get; }

        public TexturePreviewViewModel TexturePreview { get; }

        public IEnumerable<TextureNodeViewModel> Textures => _textures;

        public IActionCommand UncheckAllCommand { get; }

        public void Dispose()
        {
            _scheduler?.Stop();
            _eventPublisher.Unregister(this);
        }

        public async Task InitializeAsync()
        {
            using (new ProcessingScope(this))
            {
                _settings = await _findTextureManagementSettingsQuery.FindAsync();
                _textures.Clear();

                TexturePreview.ShowEmpty();
                if (_settings.SourceDirectory.IsNullOrEmpty())
                {
                    return;
                }

                IEnumerable<TextureFile> files = null;
                await _actionBuilder.Dispatch(new LoadTextureFilesCommand(_settings.SourceDirectory))
                    .CompleteFor<TextureFilesLoadedEvent>((context, @event) => files = @event.Files)
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                _textures.AddRange(files
                    .OrderBy(file => file.Length)
                    .ThenBy(file => file.FilePath)
                    .Select(file => new TextureNodeViewModel(file, this, _filePicker, TexturePreview)));
            }

            Rebind();
        }

        public void Rebind()
        {
            AbortCommand.Rebind();
            CheckAllCommand.Rebind();
            ConvertMultiCommand.Rebind();
            ConvertSingleCommand.Rebind();
            MoveToDestinationDirectoryCommand.Rebind();
            MoveToSourceDirectoryCommand.Rebind();
            PickSourceDirectoryCommand.Rebind();
            PickDestinationDirectoryCommand.Rebind();
            UncheckAllCommand.Rebind();
        }

        async Task IEventHandler<UpdateStatusEvent>.HandleAsync(UpdateStatusEvent @event)
        {
            if (@event.Target == Target.TextureManagement)
            {
                StatusViewModel.Set(@event.Type, @event.Text);
            }

            await Task.Yield();
        }

        private void AbortConversion()
        {
            _scheduler?.Stop();
        }

        private void CheckAll()
        {
            foreach (var node in _textures)
            {
                node.IsPicked = true;
            }
        }

        private async Task ConvertMultiAsync()
        {
            if (!IsMultiConversionEnabled())
            {
                return;
            }

            if (_isProcessing != false)
            {
                _isProcessing = null;
                _scheduler?.Stop();

                return;
            }

            using (new ProcessingScope(this))
            {
                var count = 0;
                var nodes = _textures.Where(node => node.IsPicked).ToArray();
                var settings = _settings.MultiConversion;

                await PublishUpdateStatusEventAsync(StatusType.Information, "TexturesConversionInitialization", nodes.Length);

                var collection = nodes.Select(node => new LambdaSchedulerTask(async () =>
                {
                    try
                    {
                        await _actionBuilder.Dispatch(CreateConvertTextureCommand(node.FileName, settings))
                            .CompleteFor<ActionSucceededEvent>()
                            .CompleteFor<ErrorOccuredEvent>()
                            .OnTimeout(async () => await PublishTimeoutEventAsync())
                            .Execute();
                    }
                    finally
                    {
                        Interlocked.Increment(ref count);
                    }

                    await PublishUpdateStatusEventAsync(StatusType.Information, "TexturesConversionProgress", count, nodes.Length);
                })).ToArray();

                _scheduler = new AsynchronousScheduler(Logger.Current, Environment.ProcessorCount);
                _scheduler.Subscribe(new SchedulerTaskCollection(collection));

                await _scheduler.StartAsync(true);
                await PublishUpdateStatusEventAsync(StatusType.Ok, "TexturesConversionCompleted");

                TexturePreview.Rebind();
            }
        }

        private async Task ConvertSingleAsync()
        {
            if (!IsSingleConversionEnabled() || (_isProcessing != false))
            {
                return;
            }

            using (new ProcessingScope(this))
            {
                await _actionBuilder.Dispatch(CreateConvertTextureCommand(TexturePreview.Name, _settings.SingleConversion))
                    .CompleteFor<ActionSucceededEvent>(async (context, @event) => await PublishUpdateStatusEventAsync(StatusType.Ok, "TextureConversionCompleted"))
                    .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                    .OnTimeout(async () => await PublishTimeoutEventAsync())
                    .Execute();

                TexturePreview.Rebind();
            }
        }

        private ConvertTextureCommand CreateConvertTextureCommand(string fileName, TextureConversionSettings settings)
        {
            return new ConvertTextureCommand(
                fileName,
                _settings.SourceDirectory,
                _settings.DestinationDirectory,
                new ApplicationModel.TextureConversionSettings
                {
                    ColorSpace = settings.ColorSpace,
                    KeepMipmaps = settings.KeepMipmaps,
                    OutputFormat = settings.OutputFormat
                });
        }

        private bool IsAbortConversionEnabled()
        {
            return _isProcessing != false;
        }

        private bool IsCheckAllEnabled()
        {
            return _textures.Any(node => !node.IsPicked);
        }

        private bool IsMultiConversionEnabled()
        {
            return (_isProcessing == false)
                   && !SourceDirectory.IsNullOrEmpty()
                   && !DestinationDirectory.IsNullOrEmpty()
                   && Textures.Any(node => node.IsPicked);
        }

        private bool IsSingleConversionEnabled()
        {
            return (_isProcessing == false)
                   && !TexturePreview.IsEmpty
                   && !SourceDirectory.IsNullOrEmpty()
                   && !DestinationDirectory.IsNullOrEmpty();
        }

        private bool IsUncheckAllEnabled()
        {
            return _textures.Any(node => node.IsPicked);
        }

        private void MoveToDestinationDirectory()
        {
            _directoryPicker.Open(DestinationDirectory);
        }

        private void MoveToSourceDirectory()
        {
            _directoryPicker.Open(SourceDirectory);
        }

        private async Task OnSingleChangedAsync()
        {
            TexturePreview.Rebind();
            await OnChangedAsync();
        }

        private async Task OnChangedAsync()
        {
            Rebind();
            await _actionBuilder.Dispatch(new UpdateTextureManagementSettingsCommand(_settings))
                .CompleteFor<ActionSucceededEvent>()
                .CompleteFor<ErrorOccuredEvent>(async (context, @event) => await PublishUnknownErrorEventAsync())
                .OnTimeout(async () => await PublishTimeoutEventAsync())
                .Execute();
        }

        private async Task PickDestinationDirectoryAsync()
        {
            var path = _directoryPicker.Pick(DestinationDirectory.IsNotNullOrEmpty()
                                                 ? DestinationDirectory
                                                 : null);

            if (path.IsNullOrEmpty())
            {
                return;
            }

            DestinationDirectory = path;
            await OnChangedAsync();
        }

        private async Task PickSourceDirectoryAsync()
        {
            var path = _directoryPicker.Pick(SourceDirectory.IsNotNullOrEmpty()
                                                 ? SourceDirectory
                                                 : null);

            if (path.IsNullOrEmpty())
            {
                return;
            }

            SourceDirectory = path;
            await OnChangedAsync();
            await InitializeAsync();
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
                    Target.TextureManagement,
                    type,
                    string.Format(Localization.Localization.Current.Get(key), arguments)));
        }

        private void UncheckAll()
        {
            foreach (var node in _textures)
            {
                node.IsPicked = false;
            }
        }
    }
}