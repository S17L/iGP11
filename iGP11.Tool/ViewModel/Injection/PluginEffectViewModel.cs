using System;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Tool.Common;
using iGP11.Tool.Shared.Plugin;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public class PluginEffectViewModel : ViewModel
    {
        private readonly IEffect _effect;
        private readonly PluginViewModel _viewModel;

        private bool _isSelectable;

        public PluginEffectViewModel(
            IEffect effect,
            PluginViewModel viewModel,
            ComponentViewModelFactory componentViewModelFactory)
        {
            _effect = effect;
            _viewModel = viewModel;

            Component = componentViewModelFactory.Create(_effect.Component);
            MoveDownCommand = new ActionCommand(async () => await MoveDownAsync(), () => true);
            MoveUpCommand = new ActionCommand(async () => await MoveUpAsync(), () => true);
            RemoveCommand = new ActionCommand(async () => await RemoveAsync(), () => true);
        }

        public IComponentViewModel Component { get; }

        public Guid Id => _effect.Id;

        public bool IsEnabled
        {
            get { return _effect.IsEnabled; }
            set { UpdateAvailabilityAsync(value); }
        }

        public bool IsSelected
        {
            get { return _isSelectable; }
            set
            {
                _isSelectable = value;
                OnPropertyChanged();
            }
        }

        public bool MoveDownAvailable => _viewModel.Effects.LastOrDefault() != this;

        public IActionCommand MoveDownCommand { get; }

        public bool MoveUpAvailable => _viewModel.Effects.FirstOrDefault() != this;

        public IActionCommand MoveUpCommand { get; }

        public IActionCommand RemoveCommand { get; }

        public void Rebind()
        {
            OnPropertyChanged(() => MoveDownAvailable);
            OnPropertyChanged(() => MoveUpAvailable);
            Component.Rebind();
            MoveDownCommand.Rebind();
            MoveUpCommand.Rebind();
            RemoveCommand.Rebind();
        }

        public EffectData Serialize()
        {
            return _effect.Serialize();
        }

        private async Task MoveDownAsync()
        {
            await _viewModel.MoveDownAsync(this);
        }

        private async Task MoveUpAsync()
        {
            await _viewModel.MoveUpAsync(this);
        }

        private async Task RemoveAsync()
        {
            await _viewModel.RemoveEffectAsync(_effect.Id);
        }

        private async void UpdateAvailabilityAsync(bool value)
        {
            await _viewModel.UpdateEffectAsync(_effect.Id, value);
        }
    }
}