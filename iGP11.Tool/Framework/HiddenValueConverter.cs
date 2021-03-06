﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace iGP11.Tool.Framework
{
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class HiddenValueConverter : MarkupExtension,
                                        IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as bool?).GetValueOrDefault()
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