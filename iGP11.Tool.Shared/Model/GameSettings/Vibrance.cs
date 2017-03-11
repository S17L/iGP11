using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentShortDescription("VibranceShortDescription")]
    [ComponentLongDescription("VibranceLongDescription")]
    [DataContract]
    public class Vibrance
    {
        private const string GeneralGroupBy = "General";
        private const string GainGroupBy = "Gain";

        [ComponentName("Blue")]
        [DataMember(Name = "gainBlue")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(2)]
        public float GainBlue { get; set; }

        [ComponentName("Green")]
        [DataMember(Name = "gainGreen")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(1)]
        public float GainGreen { get; set; }

        [ComponentName("Enabled")]
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [ComponentName("Red")]
        [DataMember(Name = "gainRed")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GainGroupBy)]
        [Order(0)]
        public float GainRed { get; set; }

        [ComponentName("Strength")]
        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float Strength { get; set; }
    }
}