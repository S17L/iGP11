using System;
using System.Windows.Input;

using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;

namespace iGP11.Tool
{
    public partial class ConfirmationWindow
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Target _invoker;

        public ConfirmationWindow(Target invoker, string title, string question)
        {
            InitializeComponent();

            Title = title;
            Question = question;
            CancelCommand = new ActionCommand(Cancel, () => true);
            SubmitCommand = new ActionCommand(Submit, () => true);

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));
        }

        public ICommand CancelCommand { get; }

        public string Question { get; }

        public ICommand SubmitCommand { get; }

        protected override async void OnClosed(EventArgs e)
        {
            await _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
            base.OnClosed(e);
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void Submit()
        {
            DialogResult = true;
            Close();
        }
    }
}