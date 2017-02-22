using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using ToolTipService = iGP11.Tool.Themes.ToolTipService;

namespace iGP11.Tool.UserControls
{
    public partial class ImageTextBlock
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ImageTextBlock), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty ImageDockProperty = DependencyProperty.Register("ImageDock", typeof(Dock), typeof(ImageTextBlock), new UIPropertyMetadata(Dock.Left));

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageTextBlock), new UIPropertyMetadata(16.0));

        public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageTextBlock), new UIPropertyMetadata(new Thickness(5, 0, 5, 0)));

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageTextBlock), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageTextBlock), new UIPropertyMetadata(16.0));

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(ImageTextBlock), new UIPropertyMetadata(new Thickness(0, 0, 5, 0)));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ImageTextBlock), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ImageTextBlock), new UIPropertyMetadata(TextWrapping.NoWrap));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ImageTextBlock), new UIPropertyMetadata(string.Empty));

        public ImageTextBlock()
        {
            InitializeComponent();

            ApiToolTip.SetBinding(
                ToolTipService.TitleProperty,
                new Binding("Title")
                {
                    Source = this
                });

            ApiToolTip.SetBinding(
                ToolTipService.DescriptionProperty,
                new Binding("Description")
                {
                    Source = this
                });
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public Dock ImageDock
        {
            get { return (Dock)GetValue(ImageDockProperty); }
            set { SetValue(ImageDockProperty, value); }
        }

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}