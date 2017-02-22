using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class LumaSharpen
    {
        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "offsetBias")]
        public float OffsetBias { get; set; }

        [DataMember(Name = "sharpeningClamp")]
        public float SharpeningClamp { get; set; }

        [DataMember(Name = "sharpeningStrength")]
        public float SharpeningStrength { get; set; }
    }
}