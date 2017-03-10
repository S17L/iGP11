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

        [ComponentName("BlueChannelStrength")]
        [DataMember(Name = "blueChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(3)]
        public float BlueChannelStrength { get; set; }

        [ComponentName("GreenChannelStrength")]
        [DataMember(Name = "greenChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float GreenChannelStrength { get; set; }

        [ComponentName("Enabled")]
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [ComponentName("RedChannelStrength")]
        [DataMember(Name = "redChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float RedChannelStrength { get; set; }

        [ComponentName("Strength")]
        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float Strength { get; set; }
    }
}