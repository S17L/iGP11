using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("Tonemap")]
    [ComponentLongDescription("TonemapLongDescription")]
    [DataContract]
    public class Tonemap
    {
        private const string FogGroupBy = "Fog";
        private const string GeneralGroupBy = "General";

        [ComponentName("Bleach")]
        [DataMember(Name = "bleach")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(3)]
        public float Bleach { get; set; }

        [ComponentName("Defog")]
        [DataMember(Name = "defog")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(4)]
        public float Defog { get; set; }

        [ComponentName("Exposure")]
        [DataMember(Name = "exposure")]
        [Editable]
        [FloatRange(-1, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float Exposure { get; set; }

        [ComponentName("FogBlue")]
        [DataMember(Name = "fogBlue")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(FogGroupBy)]
        [Order(2)]
        public float FogBlue { get; set; } = 1;

        [ComponentName("FogGreen")]
        [DataMember(Name = "fogGreen")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(FogGroupBy)]
        [Order(1)]
        public float FogGreen { get; set; }

        [ComponentName("FogRed")]
        [DataMember(Name = "fogRed")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(FogGroupBy)]
        [Order(0)]
        public float FogRed { get; set; }

        [ComponentName("Gamma")]
        [DataMember(Name = "gamma")]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float Gamma { get; set; } = 1;

        [ComponentName("Saturation")]
        [DataMember(Name = "saturation")]
        [Editable]
        [FloatRange(-1, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float Saturation { get; set; }
    }
}