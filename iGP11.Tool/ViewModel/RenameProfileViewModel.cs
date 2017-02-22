using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using iGP11.Library;
using iGP11.Tool.Common;

namespace iGP11.Tool.ViewModel
{
    public class RenameProfileViewModel : ViewModel,
                                          INotifyDataErrorInfo
    {
        private IEnumerable<string> _errors = Enumerable.Empty<string>();
        private string _profileName;

        public RenameProfileViewModel(string profileName)
        {
            _profileName = profileName;

            CancelCommand = new ActionCommand(Close, () => true);
            SubmitCommand = new ActionCommand(Submit, () => _errors.IsNullOrEmpty());
            Evaluate();
        }

        public delegate void CancelEventHandler();

        public delegate void SubmitEventHandler(string name);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event CancelEventHandler OnCancelled;

        public event SubmitEventHandler OnSubmitted;

        public IActionCommand CancelCommand { get; }

        public bool HasErrors => _errors.Any();

        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName == value)
                {
                    return;
                }

                _profileName = value;

                Evaluate();
                OnPropertyChanged();
            }
        }

        public IActionCommand SubmitCommand { get; }

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors;
        }

        private void Close()
        {
            OnCancelled?.Invoke();
        }

        private void Evaluate()
        {
            _errors = _profileName.IsNullOrEmpty()
                          ? new[] { "value is required" }
                          : Enumerable.Empty<string>();

            OnPropertyChanged(() => HasErrors);
            OnPropertyChanged(() => ProfileName);
            SubmitCommand.Rebind();
            CancelCommand.Rebind();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
        }

        private void Submit()
        {
            OnSubmitted?.Invoke(_profileName);
        }
    }
}