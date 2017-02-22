using System.Collections.Generic;

namespace iGP11.Library.Component
{
    public interface IComponent : IProperty
    {
        IEnumerable<IProperty> Properties { get; }
    }
}