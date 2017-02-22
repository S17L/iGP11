using System;
using System.Runtime.InteropServices;

namespace iGP11.Tool.Infrastructure.External.Model
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Texture
    {
        public readonly IntPtr Buffer;
        public readonly ulong Size;
        public readonly TextureMetadata Metadata;
    }
}