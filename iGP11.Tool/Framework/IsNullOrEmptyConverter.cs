using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using iGP11.Library;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class IsNullOrEmptyConverter : MarkupExtension,
                                          IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).IsNullOrEmpty();
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