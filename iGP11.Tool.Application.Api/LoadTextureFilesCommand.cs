using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class LoadTextureFilesCommand
    {
        public LoadTextureFilesCommand(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        [DataMember(Name = "directoryPath", IsRequired = true)]
        public string DirectoryPath { get; private set; }
    }
}