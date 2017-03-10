using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model.GameSettings;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    [KnownType(typeof(ProxySettings))]
    public class ProxyPluginSettings
    {
        [DataMember(Name = "direct3D11Settings", IsRequired = true)]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "pluginType", IsRequired = true)]
        public PluginType PluginType { get; set; }
    }
}