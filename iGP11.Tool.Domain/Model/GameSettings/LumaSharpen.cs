using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class LumaSharpen
    {
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "offsetBias")]
        [Editable]
        [FloatRange(0, 6)]
        public float OffsetBias { get; set; }

        [DataMember(Name = "sharpeningClamp")]
        [Editable]
        [FloatRange(0, 1)]
        public float SharpeningClamp { get; set; }

        [DataMember(Name = "sharpeningStrength")]
        [Editable]
        [FloatRange(0.1f, 10)]
        public float SharpeningStrength { get; set; }
    }
}