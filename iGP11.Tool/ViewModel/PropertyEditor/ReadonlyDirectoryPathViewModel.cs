using System.Collections.Generic;

using iGP11.Library.Component;
using iGP11.Tool.Common;

namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public class ReadonlyDirectoryPathViewModel : PropertyViewModel<string>
    {
        private readonly IDirectoryPicker _directoryPicker;
        private readonly IProperty _property;

        private IActionCommand _moveToDirectoryPathCommand;

        public ReadonlyDirectoryPathViewModel(IGenericProperty<string> property, IDirectoryPicker directoryPicker, IEqualityComparer<string> equalityComparer, IPropertyConverter<string> converter)
            : base(property, equalityComparer, converter)
        {
            _property = property;
            _directoryPicker = directoryPicker;
        }

        public string FormattedProperty => _property.ToString();

        public IActionCommand MoveToDirectoryPathCommand
        {
            get { return _moveToDirectoryPathCommand ?? (_moveToDirectoryPathCommand = new ActionCommand(() => _directoryPicker.Open(FormattedProperty), () => !HasErrors)); }
        }

        public override void Rebind()
        {
            base.Rebind();
            MoveToDirectoryPathCommand.Rebind();
            OnPropertyChanged(() => FormattedProperty);
        }
    }
}