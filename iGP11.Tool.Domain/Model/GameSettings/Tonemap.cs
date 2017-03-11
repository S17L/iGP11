using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Tonemap
    {
        [DataMember(Name = "bleach")]
        [Editable]
        [FloatRange(0, 1)]
        public float Bleach { get; set; }

        [DataMember(Name = "defog")]
        [Editable]
        [FloatRange(0, 1)]
        public float Defog { get; set; }

        [DataMember(Name = "fogBlue")]
        [Editable]
        [FloatRange(0, 1)]
        public float FogBlue { get; set; }

        [DataMember(Name = "fogGreen")]
        [Editable]
        [FloatRange(0, 1)]
        public float FogGreen { get; set; }

        [DataMember(Name = "fogRed")]
        [Editable]
        [FloatRange(0, 1)]
        public float FogRed { get; set; }

        [DataMember(Name = "exposure")]
        [Editable]
        [FloatRange(-1, 1)]
        public float Exposure { get; set; }

        [DataMember(Name = "gamma")]
        [Editable]
        [FloatRange(0, 2)]
        public float Gamma { get; set; }

        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "saturation")]
        [Editable]
        [FloatRange(-1, 1)]
        public float Saturation { get; set; }
    }
}