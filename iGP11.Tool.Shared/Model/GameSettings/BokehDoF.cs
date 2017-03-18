using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Model.GameSettings.Validation;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [BokehDoFValidator]
    [ComponentName("BokehDoF")]
    [ComponentShortDescription("BokehDoFShortDescription")]
    [ComponentLongDescription("BokehDoFLongDescription")]
    [DataContract]
    public class BokehDoF
    {
        private const string BlurGroupedBy = "Blur";
        private const string ChromaticAberrationGroupedBy = "ChromaticAberration";
        private const string GeneralGroupedBy = "General";
        private const string ShapeGroupedBy = "Shape";

        [ComponentName("BlurStrength")]
        [DataMember(Name = "blurStrength")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(BlurGroupedBy)]
        [Order(1)]
        public float BlurStrength { get; set; } = 1;

        [ComponentName("Fringe")]
        [DataMember(Name = "chromaticAberrationFringe")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(ChromaticAberrationGroupedBy)]
        [Order(1)]
        public float ChromaticAberrationFringe { get; set; } = 0.5f;

        [DataMember(Name = "depthMaximum")]
        [ComponentName("DepthMaximum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(1)]
        public float DepthMaximum { get; set; } = 1;

        [DataMember(Name = "depthMinimum")]
        [ComponentName("DepthMinimum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(0)]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "depthRateGain")]
        [ComponentName("DepthRateGain")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(2)]
        public float DepthRateGain { get; set; } = 2;

        [ComponentName("Enabled")]
        [DataMember(Name = "isBlurEnabled")]
        [Editable]
        [GroupedBy(BlurGroupedBy)]
        [Order(0)]
        public bool IsBlurEnabled { get; set; }

        [ComponentName("Enabled")]
        [DataMember(Name = "isChromaticAberrationEnabled")]
        [Editable]
        [GroupedBy(ChromaticAberrationGroupedBy)]
        [Order(0)]
        public bool IsChromaticAberrationEnabled { get; set; }

        [DataMember(Name = "isPreservingShape")]
        [ComponentName("PreserveShape")]
        [Editable]
        [GroupedBy(ShapeGroupedBy)]
        [Order(0)]
        public bool IsPreservingShape { get; set; }

        [DataMember(Name = "luminescenceMaximum")]
        [ComponentName("LuminescenceMaximum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(4)]
        public float LuminescenceMaximum { get; set; } = 1;

        [DataMember(Name = "luminescenceMinimum")]
        [ComponentName("LuminescenceMinimum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(3)]
        public float LuminescenceMinimum { get; set; } = 0.1f;

        [DataMember(Name = "luminescenceRateGain")]
        [ComponentName("LuminescenceRateGain")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(GeneralGroupedBy)]
        [Order(5)]
        public float LuminescenceRateGain { get; set; } = 2;

        [ComponentName("ShapeRotation")]
        [DataMember(Name = "shapeRotation")]
        [Editable]
        [FloatRange(0, 30)]
        [GroupedBy(ShapeGroupedBy)]
        [Order(3)]
        public float ShapeRotation { get; set; }

        [DataMember(Name = "shapeSize")]
        [ComponentName("ShapeSize")]
        [Editable]
        [GroupedBy(ShapeGroupedBy)]
        [Order(1)]
        [UintRange(1, 64)]
        public uint ShapeSize { get; set; } = 3;

        [ComponentName("ShapeStrength")]
        [DataMember(Name = "shapeStrength")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(ShapeGroupedBy)]
        [Order(2)]
        public float ShapeStrength { get; set; } = 1;
    }
}