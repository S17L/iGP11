using System.Collections.Generic;
using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Plugin;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Direct3D11Settings
    {
        [Complex]
        [DataMember(Name = "depthBuffer", IsRequired = true)]
        [Editable]
        public DepthBuffer DepthBuffer { get; set; }

        [DataMember(Name = "effects", IsRequired = true)]
        public IList<EffectData> Effects { get; set; }

        [Complex]
        [DataMember(Name = "pluginSettings", IsRequired = true, EmitDefaultValue = true)]
        [Editable]
        public Direct3D11PluginSettings PluginSettings { get; set; }

        [Complex]
        [DataMember(Name = "textures", IsRequired = true, EmitDefaultValue = true)]
        [Editable]
        [NoValidation]
        public Textures Textures { get; set; }
    }
}