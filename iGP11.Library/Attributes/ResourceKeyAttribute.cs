using System;

namespace iGP11.Library.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class ResourceKeyAttribute : Attribute
    {
        public ResourceKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}