using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using iGP11.Tool.Common;

namespace iGP11.Tool.UserControls
{
    public class TileControl : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(TileControl));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(TileControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed", typeof(bool?), typeof(TileControl), new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsScrollViewerEnabledProperty = DependencyProperty.Register("IsScrollViewerEnabled", typeof(bool), typeof(TileControl));

        static TileControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TileControl), new FrameworkPropertyMetadata(typeof(TileControl)));
        }

        public TileControl()
        {
            ChangeVisibilityCommand = new ActionCommand(() => IsCollapsed = !IsCollapsed, () => true);
        }

        public IActionCommand ChangeVisibilityCommand { get; }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public bool? IsCollapsed
        {
            get { return (bool?)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        public bool IsScrollViewerEnabled
        {
            get { return (bool)GetValue(IsScrollViewerEnabledProperty); }
            set { SetValue(IsScrollViewerEnabledProperty, value); }
        }
    }
}