using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using iGP11.Tool.Application;
using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Infrastructure.External
{
    public class TextureService : ITextureService
    {
        public Texture Convert(string filePath, TextureConversionSettings settings)
        {
            Model.Texture texture;
            var textureConverstionSettings = new Model.TextureConversionSettings(
                settings.MaxHeight,
                settings.MaxWidth,
                (uint)settings.OutputFormat,
                (uint)settings.ColorSpace,
                settings.KeepMipmaps,
                (uint)settings.FileType);

            var result = ImagingDriver.ConvertTexture(filePath, textureConverstionSettings, out texture);
            if ((result < 0) || (texture.Buffer == IntPtr.Zero))
            {
                return null;
            }

            try
            {
                var metadata = new TextureMetadata
                {
                    Height = texture.Metadata.Height,
                    Width = texture.Metadata.Width,
                    MipmapsCount = texture.Metadata.MipmapsCount,
                    Format = (DxgiFormat)texture.Metadata.DxgiFormat
                };

                var output = new Texture
                {
                    Buffer = new byte[texture.Size],
                    Metadata = metadata
                };

                Marshal.Copy(texture.Buffer, output.Buffer, 0, output.Buffer.Length);

                return output;
            }
            finally
            {
                ImagingDriver.ReleaseTextureBuffer(texture.Buffer);
            }
        }

        public TextureMetadata GetMetadata(string filePath)
        {
            Model.TextureMetadata metadata;
            var result = ImagingDriver.GetTextureMetadata(filePath, out metadata);

            return result >= 0
                       ? new TextureMetadata
                       {
                           Height = metadata.Height,
                           Width = metadata.Width,
                           MipmapsCount = metadata.MipmapsCount,
                           Format = (DxgiFormat)metadata.DxgiFormat
                       }
                       : null;
        }

        public IEnumerable<TextureFile> GetTextureFiles(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath, "*.dds", SearchOption.TopDirectoryOnly)
                .Select(filePath =>
                {
                    var information = new FileInfo(filePath);
                    return new TextureFile(information.Name, filePath, information.Length);
                })
                .ToArray();
        }
    }
}