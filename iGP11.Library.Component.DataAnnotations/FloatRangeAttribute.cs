using System;
using System.Collections.Generic;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class FloatRangeAttribute : Attribute,
                                       IValidator<float>
    {
        private readonly float _max;
        private readonly float _min;

        public FloatRangeAttribute(float min = float.MinValue, float max = float.MaxValue)
        {
            _min = min;
            _max = max;
        }

        public IEnumerable<Localizable> Validate(IComponentContext<float> context, IValidationContext validationContext, float value)
        {
            if ((value < _min) || (value > _max))
            {
                yield return new Localizable(
                    "ValueOutsideRange",
                    Localizable.NotLocalizable(_min),
                    Localizable.NotLocalizable(_max));
            }
        }
    }
}