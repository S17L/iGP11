using System;

namespace iGP11.Library.Component
{
    public class BoolPropertyConverter : IPropertyConverter<bool>
    {
        public bool ConvertFrom(object value)
        {
            bool output;
            if (!bool.TryParse(Convert.ToString(value), out output))
            {
                output = false;
            }

            return output;
        }

        public object ConvertTo(bool value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            bool output;
            return bool.TryParse(Convert.ToString(value), out output);
        }
    }
}