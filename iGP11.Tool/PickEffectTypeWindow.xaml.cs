using System;
using System.Collections.Generic;

using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.Shared.Plugin;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public partial class PickEffectTypeWindow
    {
        private readonly Target _invoker;
        private readonly IEventPublisher _publisher;
        private readonly PickEffectTypeViewModel _viewModel;

        public PickEffectTypeWindow(Target invoker, IEnumerable<EffectType> effectTypes)
        {
            InitializeComponent();

            _invoker = invoker;
            _publisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _publisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));

            _viewModel = new PickEffectTypeViewModel(effectTypes);
            _viewModel.OnCancelled += Cancel;
            _viewModel.OnSubmitted += Submit;

            DataContext = _viewModel;
        }

        public EffectType EffectType => _viewModel.EffectType;

        protected override async void OnClosed(EventArgs e)
        {
            await _publisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, true));
            base.OnClosed(e);
        }

        private void Cancel()
        {
            DialogResult = false;
            Close();
        }

        private void Submit(EffectType effect)
        {
            DialogResult = true;
            Close();
        }
    }
}