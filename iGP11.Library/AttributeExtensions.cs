using System;
using System.Linq;

using iGP11.Library.Attributes;

namespace iGP11.Library
{
    public static class AttributeExtensions
    {
        public static int? GetDefaultValue(this Type @enum)
        {
            if (!@enum.IsEnum)
            {
                throw new ArgumentException($"type: {@enum} is not an enum", nameof(@enum));
            }

            var attributes = @enum.GetCustomAttributes(typeof(DefaultValueAttribute), false)
                .Cast<DefaultValueAttribute>()
                .ToArray();

            return attributes.Any()
                       ? (int?)attributes[0].Value
                       : null;
        }

        public static string GetResourceKey(this Enum @enum)
        {
            var attributes = @enum.GetType()
                .GetField(@enum.ToString())
                .GetCustomAttributes(typeof(ResourceKeyAttribute), false)
                .Cast<ResourceKeyAttribute>()
                .ToArray();

            return attributes.Any()
                       ? attributes[0].Key
                       : @enum.ToString();
        }
    }
}