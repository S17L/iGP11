using System.Runtime.Serialization;

namespace iGP11.Library
{
    [DataContract]
    public class AssemblyInformation
    {
        public AssemblyInformation(
            string title,
            string description,
            string company,
            string product,
            string copyright,
            string version,
            string fileVersion,
            string informationalVersion,
            string displayVersion)
        {
            Title = title;
            Description = description;
            Company = company;
            Product = product;
            Copyright = copyright;
            Version = version;
            FileVersion = fileVersion;
            InformationalVersion = informationalVersion;
            DisplayVersion = displayVersion;
        }

        [DataMember(Name = "company")]
        public string Company { get; private set; }

        [DataMember(Name = "copyright")]
        public string Copyright { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "displayVersion")]
        public string DisplayVersion { get; private set; }

        [DataMember(Name = "fileVersion")]
        public string FileVersion { get; private set; }

        [DataMember(Name = "informationalVersion")]
        public string InformationalVersion { get; private set; }

        [DataMember(Name = "product")]
        public string Product { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "version")]
        public string Version { get; private set; }
    }
}