using System;
using System.Collections.Generic;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class UintRangeAttribute : Attribute,
                                      IValidator<uint>
    {
        private readonly uint _max;
        private readonly uint _min;

        public UintRangeAttribute(uint min = uint.MinValue, uint max = uint.MaxValue)
        {
            _min = min;
            _max = max;
        }

        public IEnumerable<Localizable> Validate(IComponentContext<uint> context, IValidationContext validationContext, uint value)
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