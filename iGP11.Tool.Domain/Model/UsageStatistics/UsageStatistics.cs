using System;
using System.Runtime.Serialization;

using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.UsageStatistics
{
    [DataContract]
    public class UsageStatistics : AggregateRoot<AggregateId>
    {
        public UsageStatistics(AggregateId id)
            : base(id)
        {
        }

        [DataMember(Name = "firstLauchTime", IsRequired = true)]
        public DateTime? FirstLaunchTime { get; set; }
    }
}