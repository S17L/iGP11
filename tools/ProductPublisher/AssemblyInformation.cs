using System.Runtime.Serialization;

namespace ProductPublisher
{
    [DataContract]
    internal class AssemblyInformation
    {
        public AssemblyInformation(string version, string productVersion, string copyright)
        {
            Version = version;
            ProductVersion = productVersion;
            Copyright = copyright;
        }

        [DataMember(Name = "copyright", IsRequired = true)]
        public string Copyright { get; private set; }

        [DataMember(Name = "productVersion", IsRequired = true)]
        public string ProductVersion { get; private set; }

        [DataMember(Name = "version", IsRequired = true)]
        public string Version { get; private set; }
    }
}