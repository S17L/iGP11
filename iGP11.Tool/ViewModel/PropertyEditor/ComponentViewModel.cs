using System;
using System.Collections.Generic;
using System.Linq;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Tool.Localization;
using iGP11.Tool.Model;

namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public class ComponentViewModel<TObject> : ViewModel,
                                               IComponentViewModel
    {
        private readonly Component<TObject> _component;

        private IEnumerable<string> _errors = Enumerable.Empty<string>();

        public ComponentViewModel(Component<TObject> component, IEnumerable<IPropertyViewModel> properties)
        {
            Properties = properties?.ToArray() ?? new IPropertyViewModel[0];
            DataSource = Properties.Where(property => property.Configuration.GroupedBy == null)
                .Union(Properties.Where(property => property.Configuration.GroupedBy != null)
                    .OrderBy(property => property.Configuration.GroupedBy.Localize())
                    .GroupBy(property => property.Configuration.GroupedBy.Localize())
                    .SelectMany(group => new object[] { new GroupedByViewModel(group.Key) }.Union(group.OrderBy(property => property.Configuration.Order)
                        .ThenBy(property => property.Name))))
                .ToArray();

            _component = component;

            foreach (var propertyViewModel in Properties)
            {
                propertyViewModel.Changed += OnPropertyChanged;
            }

            Validate();
        }

        public event StateChangedEventHandler Changed;

        public event ValidationTriggeredEventHandler ValidationTriggered;

        public IEnumerable<object> DataSource { get; }

        public string ErrorBody => string.Join(Environment.NewLine, _errors);

        public int ErrorCount => _errors.Count();

        public IEnumerable<string> Errors => _errors.ToArray();

        public string ErrorTitle => _errors.IsNullOrEmpty()
                                        ? Localization.Localization.Current.Get("NoValidationErrors")
                                        : string.Format(Localization.Localization.Current.Get("ErrorCount"), _errors.Count());

        public bool HasErrors => _errors.Any();

        public string LongDescription => _component.Configuration.LongDescription.Localize();

        public string Name => _component.Name.Localize();

        public IEnumerable<IPropertyViewModel> Properties { get; }

        public string ShortDescription => _component.Configuration.ShortDescription.Localize();

        public void Rebind()
        {
            foreach (var viewModel in Properties)
            {
                viewModel.Rebind();
            }

            Validate();
        }

        private IEnumerable<string> GetDataValidationErrors()
        {
            return Properties.SelectMany(viewModel => viewModel.GetDataValidationErrors());
        }

        private void OnPropertyChanged(IPropertyViewModel propertyViewModel, EventArgs eventArgs)
        {
            Validate();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void Validate()
        {
            _errors = _component.Validate()
                .Select(validationResult => validationResult.Localize())
                .Union(GetDataValidationErrors())
                .RemoveEmpty()
                .Distinct()
                .ToCollection();

            ValidationTriggered?.Invoke(this, new ValidationResultEventArgs(_errors.ToArray()));

            OnPropertyChanged(() => ErrorBody);
            OnPropertyChanged(() => ErrorCount);
            OnPropertyChanged(() => ErrorTitle);
            OnPropertyChanged(() => Errors);
            OnPropertyChanged(() => HasErrors);
        }
    }
}