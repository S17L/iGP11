using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class Vibrance
    {
        [DataMember(Name = "blueChannelStrength")]
        public float BlueChannelStrength { get; set; }

        [DataMember(Name = "greenChannelStrength")]
        public float GreenChannelStrength { get; set; }

        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "redChannelStrength")]
        public float RedChannelStrength { get; set; }

        [DataMember(Name = "strength")]
        public float Strength { get; set; }
    }
}