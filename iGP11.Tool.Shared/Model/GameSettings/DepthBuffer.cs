using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Shared.Model.GameSettings.Validation;

namespace iGP11.Tool.Shared.Model.GameSettings
{
    [ComponentShortDescription("DepthBufferShortDescription")]
    [ComponentLongDescription("DepthBufferLongDescription")]
    [DataContract]
    [DepthBufferValidator]
    public class DepthBuffer
    {
        private const string LimitGroupBy = "Limit";
        private const string LinearGroupBy = "Linear";

        [DataMember(Name = "depthMaximum", IsRequired = true)]
        [ComponentName("DepthMaximum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(LimitGroupBy)]
        [Order(2)]
        public float DepthMaximum { get; set; }

        [DataMember(Name = "depthMinimum", IsRequired = true)]
        [ComponentName("DepthMinimum")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(LimitGroupBy)]
        [Order(1)]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "isLimitEnabled", IsRequired = true)]
        [ComponentName("Enabled")]
        [Editable]
        [GroupedBy(LimitGroupBy)]
        [Order(0)]
        public bool IsLimitEnabled { get; set; }

        [DataMember(Name = "linearZFar", IsRequired = true)]
        [ComponentName("DistanceFar")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(LinearGroupBy)]
        [Order(1)]
        public float LinearZFar { get; set; }

        [DataMember(Name = "linearZNear", IsRequired = true)]
        [ComponentName("DistanceNear")]
        [Editable]
        [FloatRange(0)]
        [GroupedBy(LinearGroupBy)]
        [Order(0)]
        public float LinearZNear { get; set; }
    }
}