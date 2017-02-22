using System;

namespace iGP11.Library.DDD
{
    [AttributeUsage(AttributeTargets.All)]
    public class AggregateIdAttribute : Attribute
    {
        public AggregateIdAttribute(string id)
        {
            Id = id != null
                     ? new AggregateId(new Guid(id))
                     : null;
        }

        public AggregateId Id { get; }
    }
}