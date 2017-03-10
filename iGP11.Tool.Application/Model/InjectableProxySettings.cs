using System.Runtime.Serialization;

using iGP11.Tool.Domain.Model.GameSettings;

namespace iGP11.Tool.Application.Model
{
    [DataContract]
    public class InjectableProxySettings
    {
        [DataMember(Name = "communicationAddress", IsRequired = true)]
        public string CommunicationAddress { get; set; }

        [DataMember(Name = "communicationPort", IsRequired = true)]
        public ushort CommunicationPort { get; set; }

        [DataMember(Name = "direct3D11PluginPath", IsRequired = true)]
        public string Direct3D11PluginPath { get; set; }

        [DataMember(Name = "gameFilePath", IsRequired = true)]
        public string GameFilePath { get; set; }

        [DataMember(Name = "gameProfile", IsRequired = true)]
        public GameProfile GameProfile { get; set; }
    }
}