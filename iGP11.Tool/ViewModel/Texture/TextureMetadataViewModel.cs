using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ViewModel.Texture
{
    public class TextureMetadataViewModel : ViewModel
    {
        private DxgiFormat _format;
        private ulong _height;
        private ulong _mipmapsCount;
        private ulong _width;

        public DxgiFormat Format
        {
            get { return _format; }
            set
            {
                _format = value;
                OnPropertyChanged();
            }
        }

        public ulong Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public ulong MipmapsCount
        {
            get { return _mipmapsCount; }
            set
            {
                _mipmapsCount = value;
                OnPropertyChanged();
            }
        }

        public ulong Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }
    }
}