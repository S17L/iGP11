using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public class PluginElementViewModel : ViewModel
    {
        private bool _isSelected;

        public PluginElementViewModel(IComponentViewModel component)
        {
            Component = component;
        }

        public IComponentViewModel Component { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}