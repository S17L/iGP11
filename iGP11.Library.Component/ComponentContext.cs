using System;
using System.Linq;
using System.Linq.Expressions;

using iGP11.Library.Component.DataAnnotations;

namespace iGP11.Library.Component
{
    public class ComponentContext<TComponent> : IComponentContext<TComponent>
    {
        private readonly IComponent _component;

        public ComponentContext(IComponent component)
        {
            _component = component;
        }

        public Localizable GetName<TMember>(Expression<Func<TComponent, TMember>> expression)
        {
            var property = _component.Properties.FirstOrDefault(entity => entity.IsApplicable(expression));
            if (property == null)
            {
                throw new ArgumentException($"expression: {expression} could not be resolved");
            }

            return property.Name;
        }
    }
}