using System;
using System.Windows.Input;

using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;

namespace iGP11.Tool
{
    public partial class InformationWindow
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Target _invoker;

        public InformationWindow(Target invoker, string title, string information)
        {
            InitializeComponent();

            Title = title;
            Information = information;
            CloseCommand = new ActionCommand(Submit, () => true);

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));
        }

        public ICommand CloseCommand { get; }

        public string Information { get; }

        protected override async void OnClosed(EventArgs e)
        {
            await _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
            base.OnClosed(e);
        }

        private void Submit()
        {
            DialogResult = true;
            Close();
        }
    }
}