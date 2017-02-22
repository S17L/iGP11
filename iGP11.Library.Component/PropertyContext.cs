using System;
using System.Linq.Expressions;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class PropertyContext<TComponent> : IComponentContext<TComponent>
    {
        private readonly IProperty _property;

        public PropertyContext(IProperty property)
        {
            _property = property;
        }

        public Localizable GetName<TMember>(Expression<Func<TComponent, TMember>> expression)
        {
            if (!_property.IsApplicable(expression))
            {
                throw new ArgumentException($"expression: {expression} could not be resolved");
            }

            return _property.Name;
        }
    }
}