using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("LiftGammaGain")]
    [ComponentShortDescription("LiftGammaGainShortDescription")]
    [ComponentLongDescription("LiftGammaGainLongDescription")]
    [DataContract]
    public class LiftGammaGain
    {
        private const string GainGroupBy = "Gain";
        private const string GammaGroupBy = "Gamma";
        private const string LiftGroupBy = "Lift";

        [ComponentName("Blue")]
        [DataMember(Name = "gainBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GainGroupBy)]
        [Order(2)]
        public float GainBlue { get; set; } = 1;

        [ComponentName("Green")]
        [DataMember(Name = "gainGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GainGroupBy)]
        [Order(1)]
        public float GainGreen { get; set; } = 1;

        [ComponentName("Red")]
        [DataMember(Name = "gainRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GainGroupBy)]
        [Order(0)]
        public float GainRed { get; set; } = 1;

        [ComponentName("Blue")]
        [DataMember(Name = "gammaBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GammaGroupBy)]
        [Order(2)]
        public float GammaBlue { get; set; } = 1;

        [ComponentName("Green")]
        [DataMember(Name = "gammaGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GammaGroupBy)]
        [Order(1)]
        public float GammaGreen { get; set; } = 1;

        [ComponentName("Red")]
        [DataMember(Name = "gammaRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(GammaGroupBy)]
        [Order(0)]
        public float GammaRed { get; set; } = 1;

        [ComponentName("Blue")]
        [DataMember(Name = "liftBlue", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(LiftGroupBy)]
        [Order(2)]
        public float LiftBlue { get; set; } = 1;

        [ComponentName("Green")]
        [DataMember(Name = "liftGreen", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(LiftGroupBy)]
        [Order(1)]
        public float LiftGreen { get; set; } = 1;

        [ComponentName("Red")]
        [DataMember(Name = "liftRed", IsRequired = true)]
        [Editable]
        [FloatRange(0, 2)]
        [GroupedBy(LiftGroupBy)]
        [Order(0)]
        public float LiftRed { get; set; } = 1;
    }
}