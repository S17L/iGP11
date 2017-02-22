using System;
using System.Runtime.Serialization;

namespace iGP11.Tool.Shared.Model
{
    [DataContract]
    public class UsageStatistics
    {
        [DataMember(Name = "firstLauchTime", IsRequired = true)]
        public DateTime? FirstLaunchTime { get; set; }
    }
}