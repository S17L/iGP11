using iGP11.Library;
using iGP11.Tool.Common;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Texture
{
    public class TextureNodeViewModel : ViewModel
    {
        private readonly IFilePicker _filePicker;
        private readonly ITexturePreviewViewer _previewViewer;
        private readonly ITextureManagementViewModel _viewModel;
        private bool _isPicked;
        private bool _isSelected;

        public TextureNodeViewModel(
            TextureFile file,
            ITextureManagementViewModel viewModel,
            IFilePicker filePicker,
            ITexturePreviewViewer previewViewer)
        {
            _viewModel = viewModel;
            _filePicker = filePicker;
            _previewViewer = previewViewer;

            FileName = file.FileName;
            FilePath = file.FilePath;
            FileLength = FileUtility.GetFileLengthAbbreviation(file.Length);
            MoveToCommand = new ActionCommand(MoveTo, () => true);
        }

        public string FileLength { get; }

        public string FileName { get; }

        public string FilePath { get; }

        public bool IsPicked
        {
            get { return _isPicked; }
            set
            {
                if (_isPicked == value)
                {
                    return;
                }

                _isPicked = value;
                _viewModel.Rebind();

                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    _previewViewer.Enqueue(FileName, FilePath);
                }

                OnPropertyChanged();
            }
        }

        public IActionCommand MoveToCommand { get; }

        public void MoveTo()
        {
            _filePicker.OpenDirectory(FilePath);
        }
    }
}