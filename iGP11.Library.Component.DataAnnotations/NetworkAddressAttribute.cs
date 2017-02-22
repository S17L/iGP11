using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace iGP11.Library.Component.DataAnnotations
{
    [AttributeUsage(AttributeTargets.All)]
    public class NetworkAddressAttribute : Attribute,
                                           IValidator<string>
    {
        public IEnumerable<Localizable> Validate(IComponentContext<string> context, IValidationContext validationContext, string value)
        {
            if ((value != null) && value.Any() && (value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4))
            {
                IPAddress address;
                if (IPAddress.TryParse(value, out address))
                {
                    yield break;
                }
            }

            yield return new Localizable("IpAddressError");
        }
    }
}