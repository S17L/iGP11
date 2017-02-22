using System.Windows;
using System.Windows.Controls;

namespace iGP11.Tool.UserControls
{
    public class ProcessingContainer : ContentControl
    {
        public static readonly DependencyProperty IsProcessingProperty = DependencyProperty.Register("IsProcessing", typeof(bool), typeof(ProcessingContainer));

        static ProcessingContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessingContainer), new FrameworkPropertyMetadata(typeof(ProcessingContainer)));
        }

        public bool IsProcessing
        {
            get { return (bool)GetValue(IsProcessingProperty); }
            set { SetValue(IsProcessingProperty, value); }
        }
    }
}