using System;

namespace iGP11.Library.Component
{
    public class UintPropertyConverter : IPropertyConverter<uint>
    {
        public uint ConvertFrom(object value)
        {
            uint output;
            if (!uint.TryParse(Convert.ToString(value), out output))
            {
                output = 0;
            }

            return output;
        }

        public object ConvertTo(uint value)
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