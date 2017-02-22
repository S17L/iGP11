using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iGP11.Tool.Themes
{
    public class FrameworkService : DependencyObject
    {
        public static DependencyProperty DarkColorProperty = DependencyProperty.RegisterAttached("DarkColor", typeof(Brush), typeof(FrameworkService), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ErrorIndicatorOffsetProperty = DependencyProperty.RegisterAttached("ErrorIndicatorOffset", typeof(Thickness), typeof(FrameworkService), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageDockProperty = DependencyProperty.RegisterAttached("ImageDock", typeof(Dock), typeof(FrameworkService), new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageHeightProperty = DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(FrameworkService), new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageHoverProperty = DependencyProperty.RegisterAttached("ImageHover", typeof(ImageSource), typeof(FrameworkService), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageMarginProperty = DependencyProperty.RegisterAttached("ImageMargin", typeof(Thickness), typeof(FrameworkService), new FrameworkPropertyMetadata(new Thickness(5, 0, 5, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(FrameworkService), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty ImageWidthProperty = DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(FrameworkService), new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty LightColorProperty = DependencyProperty.RegisterAttached("LightColor", typeof(Brush), typeof(FrameworkService), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(FrameworkService), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty StateProperty = DependencyProperty.RegisterAttached("State", typeof(bool?), typeof(FrameworkService), new FrameworkPropertyMetadata((bool?)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static DependencyProperty TextMarginProperty = DependencyProperty.RegisterAttached("TextMargin", typeof(Thickness), typeof(FrameworkService), new FrameworkPropertyMetadata(new Thickness(0, 0, 5, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        public static Brush GetDarkColor(DependencyObject dependencyObject)
        {
            return (Brush)dependencyObject.GetValue(DarkColorProperty);
        }

        public static Thickness GetErrorIndicatorOffset(DependencyObject dependencyObject)
        {
            return (Thickness)dependencyObject.GetValue(StateProperty);
        }

        public static ImageSource GetImage(DependencyObject dependencyObject)
        {
            return (ImageSource)dependencyObject.GetValue(ImageProperty);
        }

        public static Dock GetImageDock(DependencyObject dependencyObject)
        {
            return (Dock)dependencyObject.GetValue(ImageDockProperty);
        }

        public static double GetImageHeight(DependencyObject dependencyObject)
        {
            return (double)dependencyObject.GetValue(ImageHeightProperty);
        }

        public static ImageSource GetImageHover(DependencyObject dependencyObject)
        {
            return (ImageSource)dependencyObject.GetValue(ImageHoverProperty);
        }

        public static Thickness GetImageMargin(DependencyObject dependencyObject)
        {
            return (Thickness)dependencyObject.GetValue(ImageMarginProperty);
        }

        public static double GetImageWidth(DependencyObject dependencyObject)
        {
            return (double)dependencyObject.GetValue(ImageWidthProperty);
        }

        public static Brush GetLightColor(DependencyObject dependencyObject)
        {
            return (Brush)dependencyObject.GetValue(LightColorProperty);
        }

        public static string GetPlaceholder(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PlaceholderProperty);
        }

        public static bool? GetState(DependencyObject dependencyObject)
        {
            return (bool?)dependencyObject.GetValue(StateProperty);
        }

        public static Thickness GetTextMargin(DependencyObject dependencyObject)
        {
            return (Thickness)dependencyObject.GetValue(TextMarginProperty);
        }

        public static void SetDarkColor(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(DarkColorProperty, value);
        }

        public static void SetErrorIndicatorOffset(DependencyObject dependencyObject, Thickness value)
        {
            dependencyObject.SetValue(StateProperty, value);
        }

        public static void SetImage(DependencyObject dependencyObject, ImageSource value)
        {
            dependencyObject.SetValue(ImageProperty, value);
        }

        public static void SetImageDock(DependencyObject dependencyObject, Dock value)
        {
            dependencyObject.SetValue(ImageDockProperty, value);
        }

        public static void SetImageHeight(DependencyObject dependencyObject, double value)
        {
            dependencyObject.SetValue(ImageHeightProperty, value);
        }

        public static void SetImageHover(DependencyObject dependencyObject, ImageSource value)
        {
            dependencyObject.SetValue(ImageHoverProperty, value);
        }

        public static void SetImageMargin(DependencyObject dependencyObject, Thickness value)
        {
            dependencyObject.SetValue(ImageMarginProperty, value);
        }

        public static void SetImageWidth(DependencyObject dependencyObject, double value)
        {
            dependencyObject.SetValue(ImageWidthProperty, value);
        }

        public static void SetLightColor(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(LightColorProperty, value);
        }

        public static void SetPlaceholder(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PlaceholderProperty, value);
        }

        public static void SetState(DependencyObject dependencyObject, bool? value)
        {
            dependencyObject.SetValue(StateProperty, value);
        }

        public static void SetTextMargin(DependencyObject dependencyObject, Thickness value)
        {
            dependencyObject.SetValue(TextMarginProperty, value);
        }
    }
}