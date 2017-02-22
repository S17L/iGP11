using System.Windows;
using System.Windows.Media;

namespace iGP11.Tool.Themes
{
    public class ToolTipService : DependencyObject
    {
        public static DependencyProperty ColorProperty = DependencyProperty.RegisterAttached("Color", typeof(Brush), typeof(ToolTipService), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached("Description", typeof(string), typeof(ToolTipService), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(ToolTipService), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static Brush GetColor(DependencyObject dependencyObject)
        {
            return (Brush)dependencyObject.GetValue(ColorProperty);
        }

        public static string GetDescription(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(DescriptionProperty) as string;
        }

        public static string GetTitle(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(TitleProperty) as string;
        }

        public static void SetColor(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ColorProperty, value);
        }

        public static void SetDescription(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(DescriptionProperty, value);
        }

        public static void SetTitle(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(TitleProperty, value);
        }
    }
}