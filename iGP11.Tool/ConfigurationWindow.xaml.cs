using System;
using System.Reflection;

using iGP11.Library;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public partial class ConfigurationWindow
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Target _invoker;
        private readonly ConfigurationViewModel _viewModel;

        public ConfigurationWindow(Target invoker)
        {
            _viewModel = DependencyResolver.Current.Resolve<ConfigurationViewModel>();

            InitializeComponent();

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));

            Title = $"{Assembly.GetEntryAssembly().GetAssemblyInformation().Product} - {Localization.Localization.Current.Get("ApplicationSettings")}";
        }

        protected override async void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if ((_viewModel == null) || !_viewModel.HasChanged)
            {
                _viewModel?.Dispose();
                if (_eventPublisher != null)
                {
                    await _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
                }

                return;
            }

            _viewModel?.Dispose();
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            await _viewModel.Initialize();
            DataContext = _viewModel;

            base.OnInitialized(e);
        }
    }
}