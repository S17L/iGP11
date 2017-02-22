using System;
using System.Linq;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Localization;

namespace iGP11.Tool
{
    public static class AttributeExtensions
    {
        public static string GetComponentLocalizableName(this Enum @enum)
        {
            var attributes = @enum.GetType()
                .GetField(@enum.ToString())
                .GetCustomAttributes(typeof(ComponentNameAttribute), false)
                .Cast<ComponentNameAttribute>()
                .ToArray();

            return attributes.Any()
                       ? attributes[0].Value.Localize()
                       : @enum.ToString();
        }
    }
}