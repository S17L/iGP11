using System;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class GroupedByAttribute : LocalizableAttribute
    {
        public GroupedByAttribute(string key, bool localizeEnabled = true)
            : base(key, localizeEnabled)
        {
        }
    }
}