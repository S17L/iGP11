using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.Events
{
    public class EditPluginComponentEvent
    {
        public EditPluginComponentEvent(IComponentViewModel component)
        {
            Component = component;
        }

        public IComponentViewModel Component { get; }
    }
}