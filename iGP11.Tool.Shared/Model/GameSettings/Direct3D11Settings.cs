using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Plugin;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [DataContract]
    public class Direct3D11Settings
    {
        [Complex]
        [DataMember(Name = "depthBuffer", IsRequired = true)]
        [Editable]
        public DepthBuffer DepthBuffer { get; set; }

        [DataMember(Name = "effects", IsRequired = true)]
        public List<EffectData> Effects { get; set; }

        [Complex]
        [DataMember(Name = "pluginSettings", IsRequired = true)]
        [Editable]
        public Direct3D11PluginSettings PluginSettings { get; set; }

        [Complex]
        [DataMember(Name = "textures", IsRequired = true)]
        [Editable]
        public Textures Textures { get; set; }
    }
}