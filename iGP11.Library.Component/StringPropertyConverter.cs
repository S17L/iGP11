using System;

namespace iGP11.Library.Component
{
    public class StringPropertyConverter : IPropertyConverter<string>
    {
        public string ConvertFrom(object value)
        {
            return Convert.ToString(value);
        }

        public object ConvertTo(string value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            return true;
        }
    }
}