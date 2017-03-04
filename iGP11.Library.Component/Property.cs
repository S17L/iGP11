using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class Property<TValue> : IGenericProperty<TValue>
    {
        private readonly Expression _expression;
        private readonly Func<TValue> _getter;
        private readonly TValue _initial;
        private readonly Action<TValue> _setter;
        private readonly IValidationContext _validationContext;
        private readonly IEnumerable<IValidator<TValue>> _validators;

        public Property(
            Expression expression,
            Localizable name,
            TValue initial,
            IPropertyConfiguration configuration,
            IValidationContext validationContext,
            Func<TValue> getter,
            Action<TValue> setter,
            IEnumerable<IValidator<TValue>> validators)
        {
            Name = name;
            Configuration = configuration;

            _expression = expression;
            _initial = initial;
            _validationContext = validationContext;
            _getter = getter;
            _setter = setter;
            _validators = validators;
        }

        public IPropertyConfiguration Configuration { get; }

        public virtual TValue FinalValue => _getter();

        public bool HasValidators => _validators.Any();

        public bool IsValid => Validate().IsNullOrEmpty();

        public Localizable Name { get; }

        public Type Type => typeof(TValue);

        public TValue Value
        {
            get { return _getter(); }
            set { _setter(value); }
        }

        public bool IsApplicable(Expression expression)
        {
            return new MappingExpressionVisitor(_expression, expression).AreEqual
                   || new MappingExpressionVisitor(expression, _expression).AreEqual;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _validators.SelectMany(validator => validator.Validate(new PropertyContext<TValue>(this), _validationContext, FinalValue))
                .Select(localizable => new ValidationResult(localizable))
                .ToArray();
        }
    }
}