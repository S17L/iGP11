using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class IsNullConverter : MarkupExtension,
                                   IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}