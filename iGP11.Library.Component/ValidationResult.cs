using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace iGP11.Library.Component
{
    public class ValidationResult : IEnumerable<Localizable>
    {
        private readonly IEnumerable<Localizable> _localizables;

        public ValidationResult(params Localizable[] localizables)
        {
            _localizables = localizables?.ToArray() ?? new Localizable[0];
        }

        public IEnumerator<Localizable> GetEnumerator()
        {
            return _localizables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}