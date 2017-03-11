using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class Denoise
    {
        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "noiseLevel")]
        [Editable]
        [FloatRange(0.01f, 1)]
        public float NoiseLevel { get; set; }

        [DataMember(Name = "blendingCoefficient")]
        [Editable]
        [FloatRange(0, 1)]
        public float BlendingCoefficient { get; set; }

        [DataMember(Name = "weightThreshold")]
        [Editable]
        [FloatRange(0, 1)]
        public float WeightThreshold { get; set; }

        [DataMember(Name = "counterThreshold")]
        [Editable]
        [FloatRange(0, 1)]
        public float CounterThreshold { get; set; }

        [DataMember(Name = "gaussianSigma")]
        [Editable]
        [FloatRange(1, 100)]
        public float GaussianSigma { get; set; }

        [DataMember(Name = "windowSize")]
        [Editable]
        [UintRange(1, 10)]
        public uint WindowSize { get; set; }
    }
}