using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(bool?), typeof(GridLength))]
    public class RowDefinitionHeightConverter : MarkupExtension,
                                                IValueConverter
    {
        public double Value { get; set; } = 1;

        public GridUnitType Type { get; set; } = GridUnitType.Auto;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)value == true ? new GridLength(Value, Type) : new GridLength(0);
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
