using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [DataContract]
    public class Direct3D11Settings
    {
        [Complex]
        [DataMember(Name = "bokehDoF", IsRequired = true)]
        [ComponentName("BokehDoF")]
        [Editable]
        [Order(4)]
        public BokehDoF BokehDoF { get; set; }

        [Complex]
        [ComponentName("Denoise")]
        [DataMember(Name = "denoise", IsRequired = true)]
        [Editable]
        [Order(3)]
        public Denoise Denoise { get; set; }

        [Complex]
        [ComponentName("DepthBuffer")]
        [DataMember(Name = "depthBuffer", IsRequired = true)]
        [Editable]
        [Order(2)]
        public DepthBuffer DepthBuffer { get; set; }

        [Complex]
        [ComponentName("LiftGammaGain")]
        [DataMember(Name = "liftGammaGain", IsRequired = true)]
        [Editable]
        [Order(5)]
        public LiftGammaGain LiftGammaGain { get; set; }

        [Complex]
        [ComponentName("LumaSharpen")]
        [DataMember(Name = "lumaSharpen", IsRequired = true)]
        [Editable]
        [Order(6)]
        public LumaSharpen LumaSharpen { get; set; }

        [Complex]
        [ComponentName("Direct3D11PluginSettings")]
        [DataMember(Name = "pluginSettings", IsRequired = true, EmitDefaultValue = true)]
        [Editable]
        [NoValidation]
        [Order(0)]
        public Direct3D11PluginSettings PluginSettings { get; set; }

        [Complex]
        [DataMember(Name = "textures", IsRequired = true, EmitDefaultValue = true)]
        [ComponentName("Textures")]
        [Editable]
        [NoValidation]
        [Order(1)]
        public Textures Textures { get; set; }

        [Complex]
        [ComponentName("Tonemap")]
        [DataMember(Name = "tonemap", IsRequired = true)]
        [Editable]
        [Order(7)]
        public Tonemap Tonemap { get; set; }

        [Complex]
        [ComponentName("Vibrance")]
        [DataMember(Name = "vibrance", IsRequired = true)]
        [Editable]
        [Order(8)]
        public Vibrance Vibrance { get; set; }
    }
}