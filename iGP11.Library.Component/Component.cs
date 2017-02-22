using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class Component<TObject> : IComponent
    {
        private readonly IEnumerable<IProperty> _allProperties;
        private readonly IEnumerable<IProperty> _properties;
        private readonly IValidationContext _validationContext;
        private readonly IEnumerable<IValidator<TObject>> _validators;

        public Component(
            TObject @object,
            Localizable name,
            IPropertyConfiguration configuration,
            IValidationContext validationContext,
            IEnumerable<IValidator<TObject>> validators,
            IEnumerable<IProperty> properties)
        {
            Object = @object;
            Name = name;
            Configuration = configuration;

            _validationContext = validationContext;
            _validators = validators;
            _properties = properties?.ToArray() ?? new IProperty[0];
            _properties = _properties.Where(property => property.Configuration.GroupedBy == null)
                .OrderBy(property => property.Configuration.Order)
                .ThenBy(property => property.Name)
                .Union(_properties.Where(property => property.Configuration.GroupedBy != null)
                    .OrderBy(property => property.Configuration.GroupedBy.Key)
                    .GroupBy(property => property.Configuration.GroupedBy.Key)
                    .SelectMany(group => group.OrderBy(property => property.Configuration.Order)
                        .ThenBy(property => property.Name)))
                .ToArray();

            _allProperties = GetProperties();
        }

        public IEnumerable<IProperty> AllProperties => _allProperties.ToArray();

        public IPropertyConfiguration Configuration { get; }

        public bool HasValidators => _validators.Any() || _properties.Any(property => property.HasValidators);

        public bool IsValid => Validate().IsNullOrEmpty();

        public Localizable Name { get; }

        public TObject Object { get; }

        public IEnumerable<IProperty> Properties => _properties.ToArray();

        public Type Type => typeof(TObject);

        public IEnumerable<string> GetDirectoryPaths()
        {
            return _allProperties.OfType<Property<string>>()
                .Where(property => property.Configuration.IsDirectoryPath)
                .Select(property => property.FinalValue)
                .ToArray();
        }

        public bool IsApplicable(Expression expression)
        {
            return _properties.Any(property => property.IsApplicable(expression));
        }

        public void Tokenize()
        {
            foreach (var property in _allProperties.OfType<Property<string>>()
                .Where(property => property.Configuration.IsTokenizable))
            {
                property.Value = property.FinalValue;
            }
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (var localizable in _validators.SelectMany(validator => validator.Validate(new ComponentContext<TObject>(this), _validationContext, Object)))
            {
                yield return new ValidationResult(localizable);
            }

            foreach (var property in _properties)
            {
                foreach (var validationResult in property.Validate())
                {
                    yield return new ValidationResult(
                        new[] { property.Name, new Localizable(": ") }
                            .Union(validationResult)
                            .ToArray());
                }
            }
        }

        private static void IterateProperties(IComponent component, ICollection<IProperty> properties)
        {
            var components = component.Properties.OfType<IComponent>().ToArray();
            foreach (var property in component.Properties.Except(components))
            {
                properties.Add(property);
            }

            foreach (var root in components)
            {
                properties.Add(root);
                IterateProperties(root, properties);
            }
        }

        private IEnumerable<IProperty> GetProperties()
        {
            var properties = new List<IProperty>();
            IterateProperties(this, properties);

            return properties;
        }
    }
}