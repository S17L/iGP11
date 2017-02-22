using System;

using iGP11.Library.Component;

namespace iGP11.Tool.Framework
{
    public class PropertyDataTypeAttribute : Attribute
    {
        public PropertyDataTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public virtual bool IsApplicable(IPropertyConfiguration configuration)
        {
            return true;
        }
    }
}