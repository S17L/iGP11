using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public sealed class StringProperty : Property<string>
    {
        private readonly ITokenReplacer _tokenReplacer;

        public StringProperty(
            Expression expression,
            Localizable name,
            string initial,
            IPropertyConfiguration configuration,
            ITokenReplacer tokenReplacer,
            IValidationContext validationContext,
            Func<string> getter,
            Action<string> setter,
            IEnumerable<IValidator<string>> validators)
            : base(expression, name, initial, configuration, validationContext, getter, setter, validators)
        {
            _tokenReplacer = tokenReplacer;
        }

        public override string FormattedValue => Value.IsNotNullOrEmpty() && Configuration.IsTokenizable
                                                     ? _tokenReplacer?.Replace(Value) ?? Value
                                                     : Value;

        public override string ToString()
        {
            return FormattedValue;
        }
    }
}