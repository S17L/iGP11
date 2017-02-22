using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.InjectionSettings
{
    [DataContract]
    public class DepthBuffer
    {
        [DataMember(Name = "depthMaximum", IsRequired = true)]
        public float DepthMaximum { get; set; }

        [DataMember(Name = "depthMinimum", IsRequired = true)]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "isLimitEnabled", IsRequired = true)]
        public bool IsLimitEnabled { get; set; }

        [DataMember(Name = "linearZFar", IsRequired = true)]
        [FloatRange(0)]
        public float LinearZFar { get; set; }

        [DataMember(Name = "linearZNear", IsRequired = true)]
        [FloatRange(0)]
        public float LinearZNear { get; set; }
    }
}