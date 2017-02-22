using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class NullVisibilityValueConverter : MarkupExtension,
                                                IValueConverter
    {
        public bool IsCollapsingOnNull { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsCollapsingOnNull)
            {
                return value != null
                           ? Visibility.Visible
                           : Visibility.Collapsed;
            }

            return value != null
                       ? Visibility.Collapsed
                       : Visibility.Visible;
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