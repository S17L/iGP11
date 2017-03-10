using System;

namespace iGP11.Tool.ViewModel
{
    public class LookupViewModel : ViewModel
    {
        private string _name;

        public LookupViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
}