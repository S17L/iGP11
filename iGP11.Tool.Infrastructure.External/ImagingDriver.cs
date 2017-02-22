using System;
using System.Runtime.InteropServices;

using iGP11.Tool.Infrastructure.External.Model;

namespace iGP11.Tool.Infrastructure.External
{
    internal static class ImagingDriver
    {
        private const CallingConvention Convention = CallingConvention.Cdecl;
        private const string ImagingLibrary = "iGP11.External.Imaging.dll";

        [DllImport(ImagingLibrary, CallingConvention = Convention)]
        internal static extern long ConvertTexture(string sourceTexturePath, TextureConversionSettings settings, out Texture texture);

        [DllImport(ImagingLibrary, CallingConvention = Convention)]
        internal static extern long GetTextureMetadata(string texturePath, out TextureMetadata textureMetadata);

        [DllImport(ImagingLibrary, CallingConvention = Convention)]
        internal static extern void ReleaseTextureBuffer(IntPtr textureBuffer);
    }
}