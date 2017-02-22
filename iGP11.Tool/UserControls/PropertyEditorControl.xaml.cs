using System;
using System.Windows;
using System.Windows.Input;

using iGP11.Tool.Model;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.UserControls
{
    public partial class PropertyEditorControl
    {
        public static readonly DependencyProperty ChangedCommandProperty = DependencyProperty.Register("ChangedCommand", typeof(ICommand), typeof(PropertyEditorControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty PropertyMarginProperty = DependencyProperty.Register("PropertyMargin", typeof(Thickness), typeof(PropertyEditorControl), new UIPropertyMetadata(new Thickness(10, 0, 0, 0)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(PropertyEditorControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ValidationTriggeredCommandProperty = DependencyProperty.Register("ValidationTriggeredCommand", typeof(ICommand), typeof(PropertyEditorControl), new UIPropertyMetadata(null));

        public PropertyEditorControl()
        {
            InitializeComponent();
            Container.DataContextChanged += GridOnDataContextChanged;
        }

        public ICommand ChangedCommand
        {
            get { return (ICommand)GetValue(ChangedCommandProperty); }
            set { SetValue(ChangedCommandProperty, value); }
        }

        public Thickness PropertyMargin
        {
            get { return (Thickness)GetValue(PropertyMarginProperty); }
            set { SetValue(PropertyMarginProperty, value); }
        }

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public ICommand ValidationTriggeredCommand
        {
            get { return (ICommand)GetValue(ValidationTriggeredCommandProperty); }
            set { SetValue(ValidationTriggeredCommandProperty, value); }
        }

        private void GridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var oldValue = dependencyPropertyChangedEventArgs.OldValue as IComponentViewModel;
            if (oldValue != null)
            {
                oldValue.Changed -= OnValueChanged;
                oldValue.ValidationTriggered -= OnValidationTriggered;
            }

            var newValue = dependencyPropertyChangedEventArgs.NewValue as IComponentViewModel;
            if (newValue != null)
            {
                newValue.Changed += OnValueChanged;
                newValue.ValidationTriggered += OnValidationTriggered;
            }
        }

        private void OnValidationTriggered(ViewModel.ViewModel viewModel, ValidationResultEventArgs eventArgs)
        {
            ValidationTriggeredCommand?.Execute(eventArgs);
        }

        private void OnValueChanged(ViewModel.ViewModel viewModel, EventArgs eventArgs)
        {
            ChangedCommand?.Execute(eventArgs);
        }
    }
}