using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Vibrance
    {
        [DataMember(Name = "blueChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        public float BlueChannelStrength { get; set; }

        [DataMember(Name = "greenChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        public float GreenChannelStrength { get; set; }

        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "redChannelStrength")]
        [Editable]
        [FloatRange(-10, 10)]
        public float RedChannelStrength { get; set; }

        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 1)]
        public float Strength { get; set; }
    }
}