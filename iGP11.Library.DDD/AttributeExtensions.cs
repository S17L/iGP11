using System;
using System.Linq;

namespace iGP11.Library.DDD
{
    public static class AttributeExtensions
    {
        public static AggregateId GetAggregateId(this Enum @enum)
        {
            var attributes = @enum.GetType()
                .GetField(@enum.ToString())
                .GetCustomAttributes(typeof(AggregateIdAttribute), false)
                .Cast<AggregateIdAttribute>()
                .ToArray();

            return attributes.Any()
                       ? attributes[0].Id
                       : null;
        }
    }
}