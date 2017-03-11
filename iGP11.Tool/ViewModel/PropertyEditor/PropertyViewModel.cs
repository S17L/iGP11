using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using iGP11.Library.Component;
using iGP11.Tool.Localization;

namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public class PropertyViewModel<TProperty> : ViewModel,
                                                IPropertyViewModel
    {
        private readonly IPropertyConverter<TProperty> _converter;
        private readonly IEqualityComparer<TProperty> _equalityComparer;
        private readonly IGenericProperty<TProperty> _property;
        private IEnumerable<string> _errors = Enumerable.Empty<string>();
        private bool _isValid = true;
        private object _objectProperty;

        public PropertyViewModel(IGenericProperty<TProperty> property, IEqualityComparer<TProperty> equalityComparer, IPropertyConverter<TProperty> converter)
        {
            _property = property;
            _equalityComparer = equalityComparer;
            _converter = converter;
            _objectProperty = _property.Value;

            Validate();
        }

        public event PropertyViewModelChangedEventHandler Changed;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IPropertyConfiguration Configuration => _property.Configuration;

        public bool HasErrors => _errors.Any();

        public bool IsEditable => _property.Configuration.IsEditable;

        public string Name => _property.Name.Localize();

        public object ObjectProperty
        {
            get { return _objectProperty; }
            set
            {
                if (_objectProperty == value)
                {
                    return;
                }

                _objectProperty = value;
                _isValid = _converter.IsValid(value);
                _property.Value = _converter.ConvertFrom(value);

                Validate();
                OnChanged();
                OnPropertyChanged();
            }
        }

        public TProperty Property
        {
            get { return _property.Value; }
            set
            {
                if (_equalityComparer.Equals(_property.Value, value))
                {
                    return;
                }

                _property.Value = value;

                Validate();
                OnChanged();
                OnPropertyChanged();
            }
        }

        public Type Type => _property.Type;

        public IEnumerable<string> GetDataValidationErrors()
        {
            if (_isValid)
            {
                yield break;
            }

            if (_property.Configuration.GroupedBy != null)
            {
                yield return $"[{_property.Configuration.GroupedBy.Localize()}] {_property.Name.Localize()}: data error";
            }
            else
            {
                yield return $"{_property.Name.Localize()}: data error";
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.ToArray();
        }

        public virtual void Rebind()
        {
            Validate();
            OnPropertyChanged(() => Property);
            OnPropertyChanged(() => ObjectProperty);
            OnPropertyChanged(() => HasErrors);
        }

        public override string ToString()
        {
            return $"{Name}: {Property}";
        }

        private void OnChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void Validate()
        {
            _errors = !_isValid
                          ? new[] { "data error" }
                          : _property.Validate().Select(validationResult => validationResult.Localize()).ToArray();

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
        }
    }
}