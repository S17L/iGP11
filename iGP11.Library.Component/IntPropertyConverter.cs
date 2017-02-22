using System;

namespace iGP11.Library.Component
{
    public class IntPropertyConverter : IPropertyConverter<int>
    {
        public int ConvertFrom(object value)
        {
            int output;
            if (!int.TryParse(Convert.ToString(value), out output))
            {
                output = 0;
            }

            return output;
        }

        public object ConvertTo(int value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            uint output;
            return uint.TryParse(Convert.ToString(value), out output);
        }
    }
}