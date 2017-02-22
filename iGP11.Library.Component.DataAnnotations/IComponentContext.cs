using System;
using System.Linq.Expressions;

namespace iGP11.Library.Component.DataAnnotations
{
    public interface IComponentContext<TComponent>
    {
        Localizable GetName<TMember>(Expression<Func<TComponent, TMember>> expression);
    }
}