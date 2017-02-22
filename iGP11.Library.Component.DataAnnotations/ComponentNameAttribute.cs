using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class ComponentNameAttribute : LocalizableAttribute
    {
        public ComponentNameAttribute(string key, bool localizeEnabled = true)
            : base(key, localizeEnabled)
        {
        }
    }
}