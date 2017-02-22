using System;
using System.Collections.Generic;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class FilePathAttribute : Attribute,
                                     IValidator<string>
    {
        public IEnumerable<Localizable> Validate(IComponentContext<string> context, IValidationContext validationContext, string value)
        {
            if (!validationContext.IsFilePathValid(value))
            {
                yield return new Localizable("FilePathError");
            }
        }
    }
}