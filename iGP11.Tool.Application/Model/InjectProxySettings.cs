using System.Runtime.Serialization;

using iGP11.Tool.Domain.Model.InjectionSettings;

namespace iGP11.Tool.Application.Model
{
    [DataContract]
    public class InjectProxySettings
    {
        [DataMember(Name = "communicationAddress", IsRequired = true)]
        public string CommunicationAddress { get; set; }

        [DataMember(Name = "communicationPort", IsRequired = true)]
        public ushort CommunicationPort { get; set; }

        [DataMember(Name = "direct3D11PluginPath", IsRequired = true)]
        public string Direct3D11PluginPath { get; set; }

        [DataMember(Name = "injectionSettings", IsRequired = true)]
        public InjectionSettings InjectionSettings { get; set; }
    }
}