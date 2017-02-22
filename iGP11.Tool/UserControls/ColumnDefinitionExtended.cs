using System.Windows;
using System.Windows.Controls;

namespace iGP11.Tool.UserControls
{
    public class ColumnDefinitionExtended : ColumnDefinition
    {
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(ColumnDefinitionExtended), new PropertyMetadata(true, OnVisibleChanged));

        static ColumnDefinitionExtended()
        {
            WidthProperty.OverrideMetadata(typeof(ColumnDefinitionExtended), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, CoerceWidth));
            MinWidthProperty.OverrideMetadata(typeof(ColumnDefinitionExtended), new FrameworkPropertyMetadata(0.0, null, CoerceMinWidth));
        }

        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        private static object CoerceMinWidth(DependencyObject obj, object value)
        {
            return ((ColumnDefinitionExtended)obj).Visible
                       ? value
                       : 0.0;
        }

        private static object CoerceWidth(DependencyObject obj, object value)
        {
            return ((ColumnDefinitionExtended)obj).Visible
                       ? value
                       : new GridLength(0);
        }

        private static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(WidthProperty);
            obj.CoerceValue(MinWidthProperty);
        }
    }
}