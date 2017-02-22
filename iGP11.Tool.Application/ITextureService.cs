using System.Collections.Generic;

using iGP11.Tool.Application.Api.Model;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application
{
    public interface ITextureService
    {
        Texture Convert(string filePath, TextureConversionSettings settings);

        TextureMetadata GetMetadata(string filePath);

        IEnumerable<TextureFile> GetTextureFiles(string directoryPath);
    }
}