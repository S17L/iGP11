using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class BokehDoF
    {
        [DataMember(Name = "blurStrength")]
        [Editable]
        [FloatRange(0)]
        public float BlurStrength { get; set; }

        [DataMember(Name = "chromaticAberrationFringe")]
        [Editable]
        [FloatRange(0)]
        public float ChromaticAberrationFringe { get; set; }

        [DataMember(Name = "depthMaximum")]
        [Editable]
        [FloatRange(0)]
        public float DepthMaximum { get; set; }

        [DataMember(Name = "depthMinimum")]
        [Editable]
        [FloatRange(0)]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "depthRateGain")]
        [Editable]
        [FloatRange(0)]
        public float DepthRateGain { get; set; }

        [DataMember(Name = "isBlurEnabled")]
        [Editable]
        public bool IsBlurEnabled { get; set; }

        [DataMember(Name = "isChromaticAberrationEnabled")]
        [Editable]
        public bool IsChromaticAberrationEnabled { get; set; }

        [DataMember(Name = "isEnabled")]
        [Editable]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "isPreservingShape")]
        [Editable]
        public bool IsPreservingShape { get; set; }

        [DataMember(Name = "luminescenceMaximum")]
        [Editable]
        [FloatRange(0)]
        public float LuminescenceMaximum { get; set; }

        [DataMember(Name = "luminescenceMinimum")]
        [Editable]
        [FloatRange(0)]
        public float LuminescenceMinimum { get; set; }

        [DataMember(Name = "luminescenceRateGain")]
        [Editable]
        [FloatRange(0)]
        public float LuminescenceRateGain { get; set; }

        [DataMember(Name = "shapeRotation")]
        [Editable]
        [FloatRange(0, 30)]
        public float ShapeRotation { get; set; }

        [DataMember(Name = "shapeSize")]
        [Editable]
        [UintRange(1, 64)]
        public uint ShapeSize { get; set; }

        [DataMember(Name = "shapeStrength")]
        [Editable]
        [FloatRange(0)]
        public float ShapeStrength { get; set; }
    }
}