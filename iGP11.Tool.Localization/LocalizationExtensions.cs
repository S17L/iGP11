using System.Collections.Generic;
using System.Linq;

using iGP11.Library;
using iGP11.Library.Component;

namespace iGP11.Tool.Localization
{
    public static class LocalizationExtensions
    {
        public static string Localize(this Localizable localizable)
        {
            if (localizable == null)
            {
                return string.Empty;
            }

            return localizable.LocalizeEnabled
                       ? string.Format(Localization.Current.Get(localizable.Key), LocalizeArguments(localizable.Arguments).ToArray())
                       : string.Format(localizable.Key, LocalizeArguments(localizable.Arguments));
        }

        public static string Localize(this ValidationResult validationResult)
        {
            var output = string.Empty;
            return validationResult != null
                       ? validationResult.Aggregate(output, (current, next) => string.Concat(current, next.Localize()))
                       : string.Empty;
        }

        private static string LocalizeArgument(Localizable localizable)
        {
            return localizable?.Localize() ?? string.Empty;
        }

        private static IEnumerable<object> LocalizeArguments(IEnumerable<Localizable> arguments)
        {
            return arguments.Select(LocalizeArgument).ToArray();
        }
    }
}