using System.Threading;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.Shared.Notification;

using SharedModel = iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Texture
{
    public class TexturePreviewViewModel : ViewModel,
                                           IProcessable,
                                           ITexturePreviewViewer
    {
        private readonly DomainActionBuilder _actionBuilder;
        private readonly IImageProvider _emptyImageProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ITextureManagementViewModel _viewModel;
        private bool? _isProcessing = false;
        private TextureMetadataViewModel _metadata;
        private string _name;
        private string _path;
        private byte[] _texture;

        public TexturePreviewViewModel(
            DomainActionBuilder actionBuilder,
            IEventPublisher eventPublisher,
            ITextureManagementViewModel viewModel,
            IImageProvider emptyImageProvider)
        {
            _actionBuilder = actionBuilder;
            _eventPublisher = eventPublisher;
            _emptyImageProvider = emptyImageProvider;
            _viewModel = viewModel;
        }

        public bool IsEmpty => _path.IsNullOrEmpty();

        public bool IsProcessing
        {
            get { return _isProcessing ?? false; }
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }

        public TextureMetadataViewModel Metadata
        {
            get { return _metadata; }
            set
            {
                _metadata = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public byte[] Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                OnPropertyChanged();
            }
        }

        public void Clear()
        {
            Texture = null;
        }

        public async void Enqueue(string fileName, string filePath)
        {
            if (fileName.IsNullOrEmpty() || filePath.IsNullOrEmpty())
            {
                ShowEmpty();
                return;
            }

            SharedModel.Texture texture = null;
            SharedModel.TextureMetadata textureMetadata = null;

            var command = new GenerateTexturePreviewCommand(filePath)
            {
                ColorSpace = _viewModel.ColorSpace
            };

            await _semaphore.WaitAsync();
            try
            {
                using (new ProcessingScope(this))
                {
                    Texture = null;

                    await _actionBuilder.Dispatch(command)
                        .CompleteFor<GeneratedTexturePreviewNotification>((context, @event) =>
                        {
                            texture = @event.Texture;
                            textureMetadata = @event.TextureMetadata;
                        })
                        .CompleteFor<ErrorOccuredNotification>()
                        .OnTimeout(async () => await PublishUpdateStatusEventAsync(StatusType.Failed, "OperationTimeout"))
                        .Execute();

                    if ((texture?.Metadata == null) || (textureMetadata == null))
                    {
                        ShowEmpty();
                        return;
                    }

                    Name = fileName;
                    Path = filePath;
                    Texture = texture.Buffer;

                    Metadata = new TextureMetadataViewModel
                    {
                        Height = textureMetadata.Height,
                        Width = textureMetadata.Width,
                        MipmapsCount = textureMetadata.MipmapsCount,
                        Format = textureMetadata.Format
                    };

                    _viewModel.Rebind();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Rebind()
        {
            Enqueue(_name, _path);
        }

        public void ShowEmpty()
        {
            Name = Localization.Localization.Current.Get("TexturePreview");
            Path = null;
            Texture = _emptyImageProvider.Get();
            _viewModel.Rebind();
        }

        private async Task PublishUpdateStatusEventAsync(StatusType type, string key, params object[] arguments)
        {
            await _eventPublisher.PublishAsync(
                new UpdateStatusEvent(
                    Target.TextureManagement,
                    type,
                    string.Format(Localization.Localization.Current.Get(key), arguments)));
        }
    }
}