﻿using System.Runtime.Serialization;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Tool.Domain.Model.GameSettings
{
    [DataContract]
    public class DepthBuffer
    {
        [DataMember(Name = "depthMaximum", IsRequired = true)]
        [Editable]
        [FloatRange(0)]
        public float DepthMaximum { get; set; }

        [DataMember(Name = "depthMinimum", IsRequired = true)]
        [Editable]
        [FloatRange(0)]
        public float DepthMinimum { get; set; }

        [DataMember(Name = "isLimitEnabled", IsRequired = true)]
        [Editable]
        public bool IsLimitEnabled { get; set; }

        [DataMember(Name = "linearZFar", IsRequired = true)]
        [Editable]
        [FloatRange(0)]
        public float LinearZFar { get; set; }

        [DataMember(Name = "linearZNear", IsRequired = true)]
        [Editable]
        [FloatRange(0)]
        public float LinearZNear { get; set; }
    }
}