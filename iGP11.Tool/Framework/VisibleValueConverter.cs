using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class VisibleValueConverter : MarkupExtension,
                                         IValueConverter
    {
        public bool IsCollapsing { get; set; } = true;

        private Visibility HiddenVisibility => IsCollapsing
                                                   ? Visibility.Collapsed
                                                   : Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool?).GetValueOrDefault()
                       ? Visibility.Visible
                       : HiddenVisibility;
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