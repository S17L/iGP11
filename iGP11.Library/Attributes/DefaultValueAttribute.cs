using System;

namespace iGP11.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class DefaultValueAttribute : Attribute
    {
        public DefaultValueAttribute(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}