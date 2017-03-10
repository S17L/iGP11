using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentShortDescription("TonemapShortDescription")]
    [ComponentLongDescription("TonemapLongDescription")]
    [DataContract]
    public class Tonemap
    {
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

        [ComponentName("DefogBlueChannelLoss")]
        [DataMember(Name = "defogBlueChannelLoss")]
        [Editable]
        [FloatRange(0, 2.55f)]
        [GroupedBy(GeneralGroupBy)]
        [Order(7)]
        public float DefogBlueChannelLoss { get; set; }

        [ComponentName("DefogGreenChannelLoss")]
        [DataMember(Name = "defogGreenChannelLoss")]
        [Editable]
        [FloatRange(0, 2.55f)]
        [GroupedBy(GeneralGroupBy)]
        [Order(6)]
        public float DefogGreenChannelLoss { get; set; }

        [ComponentName("DefogRedChannelLoss")]
        [DataMember(Name = "defogRedChannelLoss")]
        [Editable]
        [FloatRange(0, 2.55f)]
        [GroupedBy(GeneralGroupBy)]
        [Order(5)]
        public float DefogRedChannelLoss { get; set; }

        [ComponentName("Exposure")]
        [DataMember(Name = "exposure")]
        [Editable]
        [FloatRange(-1, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float Exposure { get; set; }

        [ComponentName("Gamma")]
        [DataMember(Name = "gamma")]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float Gamma { get; set; }

        [ComponentName("Enabled")]
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [ComponentName("Saturation")]
        [DataMember(Name = "saturation")]
        [Editable]
        [FloatRange(-1, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float Saturation { get; set; }
    }
}