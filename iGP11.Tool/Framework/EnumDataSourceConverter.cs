using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Model;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumDataSourceConverter : MarkupExtension,
                                           IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return Enum.GetValues(value.GetType())
                .Cast<Enum>()
                .Where(IsEditable)
                .Select(@enum => new ValueDescription
                {
                    Value = @enum,
                    Description = @enum.GetComponentLocalizableName()
                })
                .ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static bool IsEditable(Enum @enum)
        {
            var attributes = @enum.GetType()
                .GetField(@enum.ToString())
                .GetCustomAttributes(typeof(EditableAttribute), false)
                .OfType<EditableAttribute>()
                .ToArray();

            return attributes.Any();
        }
    }
}