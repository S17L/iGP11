using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class LiftGammaGain
    {
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "liftRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float LiftRed { get; set; }

        [DataMember(Name = "liftGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float LiftGreen { get; set; }

        [DataMember(Name = "liftBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float LiftBlue { get; set; }

        [DataMember(Name = "gammaRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GammaRed { get; set; }

        [DataMember(Name = "gammaGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GammaGreen { get; set; }

        [DataMember(Name = "gammaBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GammaBlue { get; set; }

        [DataMember(Name = "gainRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GainRed { get; set; }

        [DataMember(Name = "gainGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GainGreen { get; set; }

        [DataMember(Name = "gainBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        public float GainBlue { get; set; }
    }
}
