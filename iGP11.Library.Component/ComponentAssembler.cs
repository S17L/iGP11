using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class ComponentAssembler
    {
        private readonly IEnumerable<Type> _knownTypes = new[]
        {
            typeof(bool),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(uint),
            typeof(ulong),
            typeof(ushort)
        };

        private readonly IValidationContext _validationContext;

        public ComponentAssembler(IValidationContext validationContext)
        {
            _validationContext = validationContext;
        }

        public Component<TObject> Assemble<TObject>(TObject @object, AssemblingContext assemblingContext = null)
        {
            return ReferenceEquals(@object, null)
                       ? null
                       : CreateComponent(@object, null, GetConfiguration(typeof(TObject), GetFormType(assemblingContext)), assemblingContext);
        }

        private static PropertyConfiguration Combine(IPropertyConfiguration first, PropertyConfiguration second)
        {
            if (first.IsEditable)
            {
                second.IsEditable = first.IsEditable;
            }

            if (first.GroupedBy != null)
            {
                second.GroupedBy = first.GroupedBy;
            }

            if (first.HasDisabledValidation)
            {
                second.HasDisabledValidation = true;
            }

            if (first.IsDirectoryPath)
            {
                second.IsDirectoryPath = true;
            }

            if (first.IsTokenizable)
            {
                second.IsTokenizable = true;
            }

            if (first.Order.HasValue)
            {
                second.Order = first.Order;
            }

            if (first.ShortDescription != null)
            {
                second.ShortDescription = first.ShortDescription;
            }

            if (first.LongDescription != null)
            {
                second.LongDescription = first.LongDescription;
            }

            return second;
        }

        private static Expression CreateExression(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var parameter = Expression.Parameter(propertyInfo.DeclaringType, "entity");
            var @delegate = typeof(Func<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);

            return Expression.Lambda(@delegate, Expression.Property(parameter, propertyInfo), parameter);
        }

        private static TAttribute GetAttribute<TAttribute>(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>()
                .FirstOrDefault();
        }

        private static IEnumerable<TAttribute> GetAttributes<TAttribute>(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttributes(typeof(TAttribute), false)
                .Cast<TAttribute>()
                .ToArray();
        }

        private static PropertyConfiguration GetConfiguration(ICustomAttributeProvider provider, FormType formType)
        {
            var editableAttribute = GetAttribute<EditableAttribute>(provider);
            var groupedByAttribute = GetAttribute<GroupedByAttribute>(provider);
            var orderAttribute = GetAttribute<OrderAttribute>(provider);
            var shortDescriptionAttribute = GetAttribute<ComponentShortDescriptionAttribute>(provider);
            var longDescriptionAttribute = GetAttribute<ComponentLongDescriptionAttribute>(provider);

            return new PropertyConfiguration
            {
                IsAccessible = editableAttribute != null,
                IsEditable = (editableAttribute?.FormType ?? FormType.None).HasFlag(formType),
                GroupedBy = groupedByAttribute?.Value,
                HasDisabledValidation = GetAttribute<NoValidationAttribute>(provider) != null,
                IsDirectoryPath = GetAttribute<DirectoryPathAttribute>(provider) != null,
                IsTokenizable = GetAttribute<TokenizableAttribute>(provider) != null,
                Order = orderAttribute?.Index,
                ShortDescription = shortDescriptionAttribute?.Value,
                LongDescription = longDescriptionAttribute?.Value
            };
        }

        private static FormType GetFormType(AssemblingContext assemblingContext)
        {
            return assemblingContext?.FormType ?? FormType.None;
        }

        private Component<TObject> CreateComponent<TObject>(TObject @object, Localizable name, IPropertyConfiguration configuration, AssemblingContext assemblingContext)
        {
            var nameAttribute = GetAttribute<ComponentNameAttribute>(@object.GetType());
            if (nameAttribute != null)
            {
                name = nameAttribute.Value;
            }

            return new Component<TObject>(
                @object,
                name,
                configuration,
                _validationContext,
                GetAttributes<IValidator<TObject>>(@object.GetType()),
                GetProperties(@object, assemblingContext).ToArray());
        }

        private IProperty CreateProperty<TProperty>(object @object, TProperty initial, PropertyInfo property, Localizable name, FormType form)
        {
            return new Property<TProperty>(
                CreateExression(property),
                name,
                initial,
                GetConfiguration(property, form),
                _validationContext,
                () => (TProperty)property.GetValue(@object),
                value => property.SetValue(@object, value),
                GetAttributes<IValidator<TProperty>>(property));
        }

        private IProperty CreateStringProperty(object @object, string initial, PropertyInfo property, Localizable name, FormType form, ITokenReplacer tokenReplacer)
        {
            return new StringProperty(
                CreateExression(property),
                name,
                initial,
                GetConfiguration(property, form),
                tokenReplacer,
                _validationContext,
                () => (string)property.GetValue(@object),
                value => property.SetValue(@object, value),
                GetAttributes<IValidator<string>>(property));
        }

        private IEnumerable<IProperty> GetProperties(object @object, AssemblingContext assemblingContext)
        {
            return @object.GetType()
                .GetProperties()
                .Select(property => GetProperty(@object, property, assemblingContext))
                .Where(property => property != null)
                .ToArray();
        }

        private IProperty GetProperty(object @object, PropertyInfo property, AssemblingContext assemblingContext)
        {
            var name = GetAttribute<ComponentNameAttribute>(property)?.Value ?? Localizable.NotLocalizable(property.Name);
            var initial = (dynamic)property.GetValue(@object);
            var form = GetFormType(assemblingContext);

            if ((GetAttribute<ComplexAttribute>(property) != null) && (initial != null))
            {
                return CreateComponent(initial, name, Combine(GetConfiguration(property, form), GetConfiguration(property.PropertyType, form)), assemblingContext);
            }

            if (property.PropertyType.IsEnum || _knownTypes.Contains(property.PropertyType))
            {
                return CreateProperty(@object, initial, property, name, form);
            }

            if (property.PropertyType == typeof(string))
            {
                return CreateStringProperty(@object, initial, property, name, form, assemblingContext?.TokenReplacer);
            }

            return null;
        }
    }
}