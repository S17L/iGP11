using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Bootstrapper
{
    [DataContract]
    public class BootstrapperConfiguration
    {
        [DataMember]
        public string ProxyFilePath { get; set; }
    }
}