using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("Vibrance")]
    [ComponentLongDescription("VibranceLongDescription")]
    [DataContract]
    public class Vibrance
    {
        private const string GainGroupBy = "Gain";
        private const string GeneralGroupBy = "General";

        [ComponentName("GainBlue")]
        [DataMember(Name = "gainBlue")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(2)]
        public float GainBlue { get; set; } = 1;

        [ComponentName("GainGreen")]
        [DataMember(Name = "gainGreen")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(1)]
        public float GainGreen { get; set; } = 1;

        [ComponentName("GainRed")]
        [DataMember(Name = "gainRed")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(0)]
        public float GainRed { get; set; } = 1;

        [ComponentName("Strength")]
        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float Strength { get; set; } = 0.3f;
    }
}