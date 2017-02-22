using System.Collections.Generic;

namespace iGP11.Library.Component.DataAnnotations
{
    public interface IValidator<TValue>
    {
        IEnumerable<Localizable> Validate(IComponentContext<TValue> context, IValidationContext validationContext, TValue value);
    }
}