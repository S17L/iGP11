using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using iGP11.Library;
using iGP11.Tool.Common;

namespace iGP11.Tool.ViewModel
{
    public class AddProfileViewModel : ViewModel,
                                       INotifyDataErrorInfo
    {
        private const int MaxLength = 64;

        private IEnumerable<string> _errors = Enumerable.Empty<string>();
        private Guid _profileId;
        private string _profileName;

        public AddProfileViewModel(IEnumerable<LookupViewModel> profiles)
        {
            Profiles = profiles.ToCollection();
            ProfileId = Profiles.First().Id;
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

        public Guid ProfileId
        {
            get { return _profileId; }
            set
            {
                _profileId = value;
                OnPropertyChanged();
            }
        }

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

        public IEnumerable<LookupViewModel> Profiles { get; }

        public IActionCommand SubmitCommand { get; }

        public IEnumerable GetErrors(string propertyName)
        {
            return nameof(ProfileName) == propertyName
                       ? _errors
                       : Enumerable.Empty<string>();
        }

        private void Close()
        {
            OnCancelled?.Invoke();
        }

        private void Evaluate()
        {
            _errors = GetErrors();

            OnPropertyChanged(() => HasErrors);
            OnPropertyChanged(() => ProfileName);
            SubmitCommand.Rebind();
            CancelCommand.Rebind();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
        }

        private IEnumerable<string> GetErrors()
        {
            if (_profileName.IsNullOrEmpty())
            {
                yield return Localization.Localization.Current.Get("ValueRequired");
            }
            else if (_profileName.Length > MaxLength)
            {
                yield return Localization.Localization.Current.Get("ValueTooLong");
            }
        }

        private void Submit()
        {
            OnSubmitted?.Invoke(_profileName);
        }
    }
}