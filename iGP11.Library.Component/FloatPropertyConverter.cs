using System;

namespace iGP11.Library.Component
{
    public class FloatPropertyConverter : IPropertyConverter<float>
    {
        public float ConvertFrom(object value)
        {
            float output;
            if (!float.TryParse(Convert.ToString(value), out output))
            {
                output = 0;
            }

            return output;
        }

        public object ConvertTo(float value)
        {
            return value;
        }

        public bool IsValid(object value)
        {
            float output;
            return float.TryParse(Convert.ToString(value), out output);
        }
    }
}