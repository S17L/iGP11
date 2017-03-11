using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Vibrance
    {
        [DataMember(Name = "gainBlue")]
        [Editable]
        [FloatRange(-10, 10)]
        public float GainBlue { get; set; }

        [DataMember(Name = "gainGreen")]
        [Editable]
        [FloatRange(-10, 10)]
        public float GainGreen { get; set; }

        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "gainRed")]
        [Editable]
        [FloatRange(-10, 10)]
        public float GainRed { get; set; }

        [DataMember(Name = "strength")]
        [Editable]
        [FloatRange(0, 1)]
        public float Strength { get; set; }
    }
}