using System.Runtime.InteropServices;

namespace iGP11.Tool.Infrastructure.External.Model
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TextureMetadata
    {
        public readonly ulong Height;
        public readonly ulong Width;
        public readonly ulong MipmapsCount;
        public readonly uint DxgiFormat;
    }
}