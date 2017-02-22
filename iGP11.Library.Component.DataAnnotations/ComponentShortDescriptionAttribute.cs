using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class ComponentShortDescriptionAttribute : LocalizableAttribute
    {
        public ComponentShortDescriptionAttribute(string key, bool localizeEnabled = true)
            : base(key, localizeEnabled)
        {
        }
    }
}