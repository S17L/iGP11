using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class Direct3D11Settings
    {
        [Complex]
        [DataMember(Name = "bokehDoF", IsRequired = true)]
        [Editable]
        public BokehDoF BokehDoF { get; set; }

        [Complex]
        [DataMember(Name = "depthBuffer", IsRequired = true)]
        [Editable]
        public DepthBuffer DepthBuffer { get; set; }

        [Complex]
        [DataMember(Name = "lumaSharpen", IsRequired = true)]
        [Editable]
        public LumaSharpen LumaSharpen { get; set; }

        [Complex]
        [DataMember(Name = "pluginSettings", IsRequired = true, EmitDefaultValue = true)]
        [Editable]
        public Direct3D11PluginSettings PluginSettings { get; set; }

        [Complex]
        [DataMember(Name = "textures", IsRequired = true, EmitDefaultValue = true)]
        [Editable]
        [NoValidation]
        public Textures Textures { get; set; }

        [Complex]
        [DataMember(Name = "tonemap", IsRequired = true)]
        [Editable]
        public Tonemap Tonemap { get; set; }

        [Complex]
        [DataMember(Name = "vibrance", IsRequired = true)]
        [Editable]
        public Vibrance Vibrance { get; set; }
    }
}