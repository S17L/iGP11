using System;
using System.Collections.Generic;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class IntRangeAttribute : Attribute,
                                     IValidator<int>
    {
        private readonly int _max;
        private readonly int _min;

        public IntRangeAttribute(int min = int.MinValue, int max = int.MaxValue)
        {
            _min = min;
            _max = max;
        }

        public IEnumerable<Localizable> Validate(IComponentContext<int> context, IValidationContext validationContext, int value)
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