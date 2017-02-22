using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.Api
{
    [DataContract]
    public class GenerateTexturePreviewCommand
    {
        public GenerateTexturePreviewCommand(string filePath)
        {
            FilePath = filePath;
        }

        [DataMember(Name = "colorSpace", IsRequired = true)]
        public Srgb ColorSpace { get; set; } = Srgb.None;

        [DataMember(Name = "filePath", IsRequired = true)]
        public string FilePath { get; private set; }

        [DataMember(Name = "maxSize", IsRequired = true)]
        public ulong MaxSize { get; set; } = 256;

        [DataMember(Name = "outputFormat", IsRequired = true)]
        public DxgiFormat OutputFormat { get; set; } = DxgiFormat.R8G8B8A8Unorm;
    }
}