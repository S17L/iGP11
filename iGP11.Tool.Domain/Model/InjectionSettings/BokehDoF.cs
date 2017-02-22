using System.Runtime.Serialization;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class BokehDoF
    {
        [DataMember(Name = "blurStrength")]
        public float BlurStrength { get; set; }

        [DataMember(Name = "chromaticAberrationFringe")]
        public float ChromaticAberrationFringe { get; set; }

        [DataMember(Name = "depthMaximum")]
        public float DepthMaximum { get; set; }

        [DataMember(Name = "depthMinimum")]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "depthRateGain")]
        public float DepthRateGain { get; set; }

        [DataMember(Name = "isBlurEnabled")]
        public bool IsBlurEnabled { get; set; }

        [DataMember(Name = "isChromaticAberrationEnabled")]
        public bool IsChromaticAberrationEnabled { get; set; }

        [DataMember(Name = "isEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name = "isPreservingShape")]
        public bool IsPreservingShape { get; set; }

        [DataMember(Name = "luminescenceMaximum")]
        public float LuminescenceMaximum { get; set; }

        [DataMember(Name = "luminescenceMinimum")]
        public float LuminescenceMinimum { get; set; }

        [DataMember(Name = "luminescenceRateGain")]
        public float LuminescenceRateGain { get; set; }

        [DataMember(Name = "shapeRotation")]
        public float ShapeRotation { get; set; }

        [DataMember(Name = "shapeSize")]
        public uint ShapeSize { get; set; }

        [DataMember(Name = "shapeStrength")]
        public float ShapeStrength { get; set; }
    }
}