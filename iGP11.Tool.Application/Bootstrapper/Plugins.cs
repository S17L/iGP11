using System.Runtime.Serialization;

namespace iGP11.Tool.Application.Bootstrapper
{
    [DataContract]
    public class Plugins
    {
        [DataMember(Name = "direct3D11", IsRequired = true)]
        public string Direct3D11 { get; set; }

        [DataMember(Name = "proxy", IsRequired = true)]
        public string Proxy { get; set; }
    }
}