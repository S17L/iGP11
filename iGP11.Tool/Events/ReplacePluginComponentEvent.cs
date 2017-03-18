using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.Events
{
    public class ReplacePluginComponentEvent
    {
        public ReplacePluginComponentEvent(IComponentViewModel oldComponent, IComponentViewModel newComponent)
        {
            OldComponent = oldComponent;
            NewComponent = newComponent;
        }

        public IComponentViewModel NewComponent { get; }

        public IComponentViewModel OldComponent { get; }
    }
}