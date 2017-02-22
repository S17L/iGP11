using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(double), typeof(double))]
    public class PercentageSizeConverter : MarkupExtension,
                                           IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetDouble(value) * GetDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private static double GetDouble(object value)
        {
            return double.Parse(System.Convert.ToString(value), CultureInfo.InvariantCulture);
        }
    }
}