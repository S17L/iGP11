﻿using System;

using iGP11.Library.EventPublisher;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public partial class RenameProfileWindow
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly Target _invoker;
        private readonly RenameProfileViewModel _viewModel;

        public RenameProfileWindow(Target invoker, string name)
        {
            InitializeComponent();

            _invoker = invoker;
            _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
            _eventPublisher.PublishAsync(new ChangeViewEnabledEvent(_invoker, false));

            _viewModel = new RenameProfileViewModel(name);
            _viewModel.OnCancelled += Cancel;
            _viewModel.OnSubmitted += Submit;
            DataContext = _viewModel;
        }

        public string ProfileName => _viewModel.ProfileName;

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

        private void Submit(string name)
        {
            DialogResult = true;
            Close();
        }
    }
}