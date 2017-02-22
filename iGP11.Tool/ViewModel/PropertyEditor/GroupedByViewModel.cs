namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public class GroupedByViewModel : ViewModel
    {
        public GroupedByViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}