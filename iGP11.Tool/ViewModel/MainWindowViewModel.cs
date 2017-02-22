using System;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ViewModel.Injection;

namespace iGP11.Tool.ViewModel
{
    public sealed class MainWindowViewModel : ViewModel,
                                              IEventHandler<ChangeViewEnabledEvent>,
                                              IEventHandler<UpdateStatusEvent>,
                                              IDisposable,
                                              IProcessable
    {
        private readonly IEventPublisher _eventPublisher;
        private bool _isEnabled = true;

        private bool _isProcessing;

        public MainWindowViewModel(MenuViewModel menuViewModel, InjectionConfigurationViewModelFactory injectionConfigurationViewModelFactory, IEventPublisher eventPublisher, ITaskRunner dispatcher)
        {
            _eventPublisher = eventPublisher;
            _eventPublisher.Register<ChangeViewEnabledEvent>(this);
            _eventPublisher.Register<UpdateStatusEvent>(this);
            StatusViewModel = new StatusViewModel(dispatcher);
            MenuViewModel = menuViewModel;
            InjectionConfigurationViewModel = injectionConfigurationViewModelFactory.Create(this);
        }

        public InjectionConfigurationViewModel InjectionConfigurationViewModel { get; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }

        public MenuViewModel MenuViewModel { get; }

        public StatusViewModel StatusViewModel { get; }

        public void Dispose()
        {
            _eventPublisher.Unregister<ChangeViewEnabledEvent>(this);
            _eventPublisher.Unregister<UpdateStatusEvent>(this);
            InjectionConfigurationViewModel.Dispose();
        }

        async Task IEventHandler<ChangeViewEnabledEvent>.HandleAsync(ChangeViewEnabledEvent @event)
        {
            if (@event.Target == Target.EntryPoint)
            {
                IsEnabled = @event.IsEnabled;
            }

            await Task.Yield();
        }

        async Task IEventHandler<UpdateStatusEvent>.HandleAsync(UpdateStatusEvent @event)
        {
            if (@event.Target == Target.EntryPoint)
            {
                StatusViewModel.Set(@event.Type, @event.Text);
            }

            await Task.Yield();
        }
    }
}