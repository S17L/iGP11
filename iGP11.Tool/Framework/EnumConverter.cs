using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumConverter : MarkupExtension,
                                 IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Enum)?.GetComponentLocalizableName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}