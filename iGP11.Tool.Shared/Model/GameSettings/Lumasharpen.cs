using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentShortDescription("LumaSharpenShortDescription")]
    [ComponentLongDescription("LumaSharpenLongDescription")]
    [DataContract]
    public class LumaSharpen
    {
        private const string GeneralGroupBy = "General";

        [ComponentName("Enabled")]
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [ComponentName("OffsetBias")]
        [DataMember(Name = "offsetBias")]
        [Editable]
        [FloatRange(0, 6)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float OffsetBias { get; set; }

        [ComponentName("SharpeningClamp")]
        [DataMember(Name = "sharpeningClamp")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float SharpeningClamp { get; set; }

        [ComponentName("SharpeningStrength")]
        [DataMember(Name = "sharpeningStrength")]
        [Editable]
        [FloatRange(0.1f, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float SharpeningStrength { get; set; }
    }
}