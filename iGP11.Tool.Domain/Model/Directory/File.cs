using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Model.Directory
{
    [DataContract]
    public class File
    {
        private readonly IFileContentLoadingPolicy _fileContentLoadingPolicy;
        private byte[] _content;

        public File(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }

        public File(string name, IFileContentLoadingPolicy fileContentLoadingPolicy)
        {
            _fileContentLoadingPolicy = fileContentLoadingPolicy;
            Name = name;
        }

        [DataMember(Name = "content")]
        public byte[] Content
        {
            get { return _content = _content ?? _fileContentLoadingPolicy.Load() ?? new byte[0]; }
            private set { _content = value; }
        }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; private set; }
    }
}