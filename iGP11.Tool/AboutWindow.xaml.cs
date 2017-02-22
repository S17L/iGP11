using System;
using System.Windows.Input;

using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ReadModel.Api;

namespace iGP11.Tool
{
    public partial class AboutWindow
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly Target _invoker;

        public AboutWindow(Target invoker)
        {
            InitializeComponent();

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));

            Title = Localization.Localization.Current.Get("About");
            CloseCommand = new ActionCommand(Submit, () => true);
        }

        public ICommand CloseCommand { get; }

        public string FeedbackEmail { get; private set; }

        public string UsedIconsUri { get; private set; }

        protected override async void OnClosed(EventArgs e)
        {
            await _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
            base.OnClosed(e);
        }

        protected override async void OnInitialized(EventArgs e)
        {
            var settings = await DependencyResolver.Current.Resolve<IFindConstantSettingsQuery>().FindAsync();

            FeedbackEmail = settings.FeedbackEmail;
            UsedIconsUri = settings.UsedIconsUri;

            base.OnInitialized(e);
        }

        private void Submit()
        {
            DialogResult = true;
            Close();
        }
    }
}