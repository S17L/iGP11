using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class ComponentLongDescriptionAttribute : LocalizableAttribute
    {
        public ComponentLongDescriptionAttribute(string key, bool localizeEnabled = true)
            : base(key, localizeEnabled)
        {
        }
    }
}