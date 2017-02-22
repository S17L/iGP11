using System;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.DDD.Action;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ReadModel.Api;
using iGP11.Tool.Shared.Model.ApplicationSettings;
using iGP11.Tool.Shared.Notification;

namespace iGP11.Tool.ViewModel
{
    public sealed class ConfigurationViewModel : ViewModel,
                                                 IEventHandler<UpdateStatusEvent>,
                                                 IDisposable
    {
        private readonly DomainActionBuilder _actionBuilder;
        private readonly IEventPublisher _eventPublisher;
        private readonly IFindConstantSettingsQuery _findConstantSettingsQuery;

        private bool _isValid;
        private ApplicationSettings _settings;
        private IActionCommand _validationTriggeredCommand;

        public ConfigurationViewModel(
            DomainActionBuilder actionBuilder,
            IEventPublisher eventPublisher,
            IFindConstantSettingsQuery findConstantSettingsQuery,
            ITaskRunner runner)
        {
            _actionBuilder = actionBuilder;
            _eventPublisher = eventPublisher;
            _eventPublisher.Register(this);
            _findConstantSettingsQuery = findConstantSettingsQuery;

            StatusViewModel = new StatusViewModel(runner);
            ChangedCommand = new ActionCommand(UpdateApplicationSettings, () => true);
        }

        ~ConfigurationViewModel()
        {
            Dispose();
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        public IActionCommand ChangedCommand { get; }

        public bool HasChanged { get; private set; }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid == value)
                {
                    return;
                }

                _isValid = value;
                OnPropertyChanged();
            }
        }

        public StatusViewModel StatusViewModel { get; }

        public IActionCommand ValidationTriggeredCommand
        {
            get { return _validationTriggeredCommand ?? (_validationTriggeredCommand = new GenericActionCommand<ValidationResultEventArgs>(ValidationTriggered, () => true)); }
        }

        public void Dispose()
        {
            _eventPublisher.Unregister(this);
        }

        public async Task Initialize()
        {
            var settings = await _findConstantSettingsQuery.FindAsync();
            ApplicationSettings = new ApplicationSettings
            {
                ApplicationCommunicationPort = settings.ApplicationCommunicationPort,
                ProxyCommunicationPort = settings.ProxyCommunicationPort
            };
        }

        async Task IEventHandler<UpdateStatusEvent>.HandleAsync(UpdateStatusEvent @event)
        {
            if (@event.Target == Target.Configurator)
            {
                StatusViewModel.Set(@event.Type, @event.Text);
            }

            await Task.Yield();
        }

        private async Task PublishUpdateStatusEvent(StatusType type, string key, params object[] arguments)
        {
            await _eventPublisher.PublishAsync(
                new UpdateStatusEvent(
                    Target.Configurator,
                    type,
                    string.Format(Localization.Localization.Current.Get(key), arguments)));
        }

        private async void UpdateApplicationSettings()
        {
            if (!_isValid)
            {
                return;
            }

            await _actionBuilder.Dispatch(new UpdateApplicationSettingsCommand(_settings.ApplicationCommunicationPort, _settings.ProxyCommunicationPort))
                .ListenFor<ActionSucceededEvent>(async (context, @event) => await context.CompleteAsync())
                .CompleteFor<ErrorOccuredEvent>()
                .Execute();

            await PublishUpdateStatusEvent(StatusType.Ok, "SettingsSaved", DateTime.Now);
            HasChanged = true;
        }

        private void ValidationTriggered(ValidationResultEventArgs eventArgs)
        {
            IsValid = eventArgs.Errors.IsNullOrEmpty();
        }
    }
}