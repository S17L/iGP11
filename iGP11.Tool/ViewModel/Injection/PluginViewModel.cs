using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Library.Component;
using iGP11.Library.EventPublisher;
using iGP11.Tool.Common;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.Shared.Plugin;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel.Injection
{
    public class PluginViewModel : ViewModel
    {
        private readonly PluginComponent _component;
        private readonly ComponentViewModelFactory _componentViewModelFactory;
        private readonly IPluginDataAccessLayer _dataAccessLayer;
        private readonly IList<PluginEffectViewModel> _effects;
        private readonly IList<PluginElementViewModel> _elements;
        private readonly INavigationService _navigationService;
        private readonly IEventPublisher _publisher;

        private bool _isValid;

        public PluginViewModel(
            IPluginDataAccessLayer dataAccessLayer,
            IEventPublisher publisher,
            INavigationService navigationService,
            ComponentViewModelFactory componentViewModelFactory)
        {
            _dataAccessLayer = dataAccessLayer;
            _publisher = publisher;
            _componentViewModelFactory = componentViewModelFactory;
            _component = _dataAccessLayer.CreateComponent();
            _navigationService = navigationService;
            _elements = new ObservableCollection<PluginElementViewModel>(_component.Elements.Select(CreatePluginElementViewModel));
            _effects = new ObservableCollection<PluginEffectViewModel>(_component.Effects.Select(CreatePluginEffectViewModel));

            AddCommand = new ActionCommand(async () => await AddEffectAsync(), () => true);
        }

        public IActionCommand AddCommand { get; }

        public IEnumerable<PluginEffectViewModel> Effects => _effects;

        public IEnumerable<PluginElementViewModel> Elements => _elements;

        public bool IsValid
        {
            get { return _isValid; }
            private set
            {
                if (_isValid == value)
                {
                    return;
                }

                _isValid = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<EffectType> SupportedEffectTypes => _component.SupportedEffectTypes;

        public async Task AddEffectAsync()
        {
            var type = _navigationService.ShowPickEffectTypeDialog(
                Target.EntryPoint,
                _component.SupportedEffectTypes);

            if (!type.HasValue)
            {
                return;
            }

            var effectViewModel = CreatePluginEffectViewModel(_dataAccessLayer.AddEffect(type.Value));
            _effects.Add(effectViewModel);

            Rebind();

            await _publisher.PublishAsync(new PluginChangedEvent());
            await _publisher.PublishAsync(new EditPluginComponentEvent(effectViewModel.Component));
        }

        public async Task MoveDownAsync(PluginEffectViewModel viewModel)
        {
            var indexOf = _effects.IndexOf(viewModel);
            if (indexOf < _effects.Count - 1)
            {
                _effects.Move(indexOf, indexOf + 1);

                UpdateEffects();
                Rebind();

                await _publisher.PublishAsync(new PluginChangedEvent());
            }
        }

        public async Task MoveUpAsync(PluginEffectViewModel viewModel)
        {
            var indexOf = _effects.IndexOf(viewModel);
            if (indexOf > 0)
            {
                _effects.Move(indexOf, indexOf - 1);

                UpdateEffects();
                Rebind();

                await _publisher.PublishAsync(new PluginChangedEvent());
            }
        }

        public void Rebind()
        {
            foreach (var viewModel in _elements.Select(element => element.Component))
            {
                viewModel.Rebind();
            }

            foreach (var viewModel in _effects.Select(effect => effect))
            {
                viewModel.Rebind();
            }
        }

        public async Task RemoveEffectAsync(Guid id)
        {
            var oldComponentViewModel = _effects.Single(effect => effect.Id == id);
            _effects.Remove(oldComponentViewModel);

            UpdateEffects();
            Rebind();

            await _publisher.PublishAsync(new PluginChangedEvent());
            await _publisher.PublishAsync(new ReplacePluginComponentEvent(
                oldComponentViewModel.Component,
                _effects.FirstOrDefault()?.Component));
        }

        public void Select(IComponentViewModel component)
        {
            foreach (var viewModel in _elements)
            {
                viewModel.IsSelected = viewModel.Component == component;
            }

            foreach (var viewModel in _effects)
            {
                viewModel.IsSelected = viewModel.Component == component;
            }
        }

        public async Task UpdateEffectAsync(Guid id, bool isEnabled)
        {
            _effects.Single(effect => effect.Id == id).IsEnabled = isEnabled;
            UpdateEffects();
            await _publisher.PublishAsync(new PluginChangedEvent());
        }

        public void UpdateEffects()
        {
            _dataAccessLayer.UpdateEffects(_effects.Select(effect => effect.Serialize()));
        }

        public void Validate()
        {
            IsValid = Elements.Select(element => element.Component)
                .Union(Effects.Select(viewModel => viewModel.Component))
                .All(viewModel => !viewModel.HasErrors);
        }

        private PluginEffectViewModel CreatePluginEffectViewModel(IEffect effect)
        {
            return new PluginEffectViewModel(effect, this, _componentViewModelFactory);
        }

        private PluginElementViewModel CreatePluginElementViewModel(IComponent component)
        {
            return new PluginElementViewModel(_componentViewModelFactory.Create(component));
        }
    }
}