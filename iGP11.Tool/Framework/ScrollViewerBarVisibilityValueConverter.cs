using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(bool), typeof(ScrollBarVisibility))]
    public class ScrollViewerBarVisibilityValueConverter : MarkupExtension,
                                                           IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool?).GetValueOrDefault()
                       ? ScrollBarVisibility.Auto
                       : ScrollBarVisibility.Disabled;
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