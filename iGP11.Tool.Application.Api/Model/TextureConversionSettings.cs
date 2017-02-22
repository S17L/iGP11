using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.Api.Model
{
    public class TextureConversionSettings
    {
        public Srgb ColorSpace { get; set; } = Srgb.None;

        public FileType FileType { get; set; } = FileType.DDS;

        public bool KeepMipmaps { get; set; } = true;

        public ulong MaxHeight { get; set; }

        public ulong MaxWidth { get; set; }

        public DxgiFormat OutputFormat { get; set; } = DxgiFormat.Bc3Unorm;
    }
}