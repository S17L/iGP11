using System;

namespace iGP11.Library.Component
{
    public class EnumPropertyConverter<TEnum> : IPropertyConverter<TEnum>
    {
        public TEnum ConvertFrom(object value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), Convert.ToString(value));
        }

        public object ConvertTo(TEnum value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            return (value != null) && value.GetType()
                       .IsEnum;
        }
    }
}