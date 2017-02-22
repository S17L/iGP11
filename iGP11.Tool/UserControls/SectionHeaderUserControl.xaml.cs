using System.Windows;
using System.Windows.Media;

namespace iGP11.Tool.UserControls
{
    public partial class SectionHeaderUserControl
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(SectionHeaderUserControl), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty FillBackgroundProperty = DependencyProperty.Register("FillBackground", typeof(Brush), typeof(SectionHeaderUserControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SectionHeaderUserControl), new UIPropertyMetadata(string.Empty));

        public SectionHeaderUserControl()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public Brush FillBackground
        {
            get { return (Brush)GetValue(FillBackgroundProperty); }
            set { SetValue(FillBackgroundProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}