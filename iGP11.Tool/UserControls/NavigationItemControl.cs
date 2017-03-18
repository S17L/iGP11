using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iGP11.Tool.UserControls
{
    public class NavigationItemControl : ContentControl
    {
        public static readonly DependencyProperty ClickedCommandParameterProperty = DependencyProperty.Register("ClickedCommandParameter", typeof(object), typeof(NavigationItemControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ClickedCommandProperty = DependencyProperty.Register("ClickedCommand", typeof(ICommand), typeof(NavigationItemControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsArrowVisibleProperty = DependencyProperty.Register("IsArrowVisible", typeof(bool), typeof(NavigationItemControl), new UIPropertyMetadata(true));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationItemControl), new UIPropertyMetadata(false));

        static NavigationItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationItemControl), new FrameworkPropertyMetadata(typeof(NavigationItemControl)));
        }

        public ICommand ClickedCommand
        {
            get { return (ICommand)GetValue(ClickedCommandProperty); }
            set { SetValue(ClickedCommandProperty, value); }
        }

        public object ClickedCommandParameter
        {
            get { return GetValue(ClickedCommandProperty); }
            set { SetValue(ClickedCommandProperty, value); }
        }

        public bool IsArrowVisible
        {
            get { return (bool)GetValue(IsArrowVisibleProperty); }
            set { SetValue(IsArrowVisibleProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
    }
}