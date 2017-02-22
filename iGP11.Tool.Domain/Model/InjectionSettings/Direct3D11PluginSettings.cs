using System.Runtime.Serialization;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class Direct3D11PluginSettings
    {
        [DataMember(Name = "profileType", IsRequired = true)]
        public Direct3D11ProfileType ProfileType { get; set; }

        [DataMember(Name = "renderingMode", IsRequired = true)]
        public RenderingMode RenderingMode { get; set; }
    }
}