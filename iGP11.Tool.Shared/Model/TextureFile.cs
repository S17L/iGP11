using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class TextureFile
    {
        public TextureFile(string fileName, string filePath, long length)
        {
            FileName = fileName;
            FilePath = filePath;
            Length = length;
        }

        [DataMember(Name = "fileName", IsRequired = true)]
        public string FileName { get; private set; }

        [DataMember(Name = "filePath", IsRequired = true)]
        public string FilePath { get; private set; }

        [DataMember(Name = "length", IsRequired = true)]
        public long Length { get; private set; }
    }
}