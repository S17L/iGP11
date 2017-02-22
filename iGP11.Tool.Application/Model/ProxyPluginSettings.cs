using System.Runtime.Serialization;

using iGP11.Tool.Domain.Model.InjectionSettings;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Application.Model
{
    [DataContract]
    [KnownType(typeof(Shared.Model.ProxySettings))]
    public class ProxyPluginSettings
    {
        [DataMember(Name = "direct3D11Settings", IsRequired = true)]
        public Direct3D11Settings Direct3D11Settings { get; set; }

        [DataMember(Name = "pluginType", IsRequired = true)]
        public PluginType PluginType { get; set; }
    }
}