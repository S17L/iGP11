using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentName("Denoise")]
    [ComponentShortDescription("DenoiseShortDescription")]
    [ComponentLongDescription("DenoiseLongDescription")]
    [DataContract]
    public class Denoise
    {
        private const string GeneralGroupBy = "General";

        [ComponentName("BlendingCoefficient")]
        [DataMember(Name = "blendingCoefficient")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(1)]
        public float BlendingCoefficient { get; set; } = 0.8f;

        [ComponentName("CounterThreshold")]
        [DataMember(Name = "counterThreshold")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(3)]
        public float CounterThreshold { get; set; } = 0.05f;

        [ComponentName("GaussianSigma")]
        [DataMember(Name = "gaussianSigma")]
        [Editable]
        [FloatRange(1, 100)]
        [GroupedBy(GeneralGroupBy)]
        [Order(4)]
        public float GaussianSigma { get; set; } = 50;

        [ComponentName("NoiseLevel")]
        [DataMember(Name = "noiseLevel")]
        [Editable]
        [FloatRange(0.01f, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(0)]
        public float NoiseLevel { get; set; } = 0.15f;

        [ComponentName("WeightThreshold")]
        [DataMember(Name = "weightThreshold")]
        [Editable]
        [FloatRange(0, 1)]
        [GroupedBy(GeneralGroupBy)]
        [Order(2)]
        public float WeightThreshold { get; set; } = 0.03f;

        [ComponentName("WindowSize")]
        [DataMember(Name = "windowSize")]
        [Editable]
        [UintRange(1, 10)]
        [GroupedBy(GeneralGroupBy)]
        [Order(5)]
        public uint WindowSize { get; set; } = 3;
    }
}