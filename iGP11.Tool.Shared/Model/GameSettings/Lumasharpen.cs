using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("LumaSharpen")]
    [ComponentShortDescription("LumaSharpenShortDescription")]
    [ComponentLongDescription("LumaSharpenLongDescription")]
    [DataContract]
    public class LumaSharpen
    {
        private const string GeneralGroupBy = "General";

        [ComponentName("Offset")]
        [DataMember(Name = "offset")]
        [Editable]
        [FloatRange(0, 6)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float Offset { get; set; } = 1;

        [ComponentName("SharpeningClamp")]
        [DataMember(Name = "sharpeningClamp")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float SharpeningClamp { get; set; } = 0.035f;

        [ComponentName("SharpeningStrength")]
        [DataMember(Name = "sharpeningStrength")]
        [Editable]
        [FloatRange(0.1f, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float SharpeningStrength { get; set; } = 1;
    }
}