using System;

namespace iGP11.Library.Component
{
    public class UshortPropertyConverter : IPropertyConverter<ushort>
    {
        public ushort ConvertFrom(object value)
        {
            ushort output;
            if (!ushort.TryParse(Convert.ToString(value), out output))
            {
                output = 0;
            }

            return output;
        }

        public object ConvertTo(ushort value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            ushort output;
            return ushort.TryParse(Convert.ToString(value), out output);
        }
    }
}