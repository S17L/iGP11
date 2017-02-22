using System.Runtime.InteropServices;

namespace iGP11.Tool.Infrastructure.External.Model
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TextureConversionSettings
    {
        public readonly ulong MaxHeight;
        public readonly ulong MaxWidth;
        public readonly uint DxgiFormat;
        public readonly uint ColorSpace;

        [MarshalAs(UnmanagedType.I1)]
        public readonly bool KeepMipMaps;

        public readonly uint FileType;

        public TextureConversionSettings(ulong maxHeight, ulong maxWidth, uint dxgiFormat, uint colorSpace, bool keepMipMaps, uint fileType)
        {
            MaxHeight = maxHeight;
            MaxWidth = maxWidth;
            DxgiFormat = dxgiFormat;
            ColorSpace = colorSpace;
            KeepMipMaps = keepMipMaps;
            FileType = fileType;
        }
    }
}