using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace iGP11.Library.Component
{
    public interface IProperty
    {
        IPropertyConfiguration Configuration { get; }

        bool IsValid { get; }

        Localizable Name { get; }

        Type Type { get; }

        bool IsApplicable(Expression expression);

        IEnumerable<ValidationResult> Validate();
    }
}