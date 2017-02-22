using System.Runtime.Serialization;

using iGP11.Tool.Application.Api.Model;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class ConvertTextureCommand
    {
        public ConvertTextureCommand(
            string fileName,
            string sourceDirectoryPath,
            string destinationDirectoryPath,
            TextureConversionSettings settings)
        {
            FileName = fileName;
            SourceDirectoryPath = sourceDirectoryPath;
            DestinationDirectoryPath = destinationDirectoryPath;
            Settings = settings;
        }

        [DataMember(Name = "destinationDirectoryPath", IsRequired = true)]
        public string DestinationDirectoryPath { get; private set; }

        [DataMember(Name = "fileName", IsRequired = true)]
        public string FileName { get; private set; }

        [DataMember(Name = "settings", IsRequired = true)]
        public TextureConversionSettings Settings { get; private set; }

        [DataMember(Name = "sourceDirectoryPath", IsRequired = true)]
        public string SourceDirectoryPath { get; private set; }
    }
}