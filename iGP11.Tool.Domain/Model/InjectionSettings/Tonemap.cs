using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class Tonemap
    {
        [DataMember(Name = "bleach")]
        public float Bleach { get; set; }

        [DataMember(Name = "defog")]
        public float Defog { get; set; }

        [DataMember(Name = "defogBlueChannelLoss")]
        public float DefogBlueChannelLoss { get; set; }

        [DataMember(Name = "defogGreenChannelLoss")]
        public float DefogGreenChannelLoss { get; set; }

        [DataMember(Name = "defogRedChannelLoss")]
        public float DefogRedChannelLoss { get; set; }

        [DataMember(Name = "exposure")]
        public float Exposure { get; set; }

        [DataMember(Name = "gamma")]
        public float Gamma { get; set; }

        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "saturation")]
        public float Saturation { get; set; }
    }
}