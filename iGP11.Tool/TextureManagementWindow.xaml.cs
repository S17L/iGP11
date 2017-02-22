using System;
using System.Reflection;

using iGP11.Library;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ViewModel.Texture;

namespace iGP11.Tool
{
    public partial class TextureManagementWindow
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Target _invoker;
        private readonly TextureManagementWindowViewModel _viewModel;

        public TextureManagementWindow(Target invoker)
        {
            _viewModel = DependencyResolver.Current.Resolve<TextureManagementWindowViewModel>();

            InitializeComponent();

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));

            DataContext = _viewModel;
            Title = $"{Assembly.GetEntryAssembly().GetAssemblyInformation().Product} - {Localization.Localization.Current.Get("TextureManagement")}";
        }

        protected override async void OnClosed(EventArgs e)
        {
            _viewModel.Dispose();
            await _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
            base.OnClosed(e);
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await _viewModel.InitializeAsync();
        }
    }
}