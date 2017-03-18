using System.Collections.Generic;
using System.Linq;

using iGP11.Library.Component;
using iGP11.Tool.Common;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.ViewModel
{
    public class ComponentViewModelFactory
    {
        private readonly IDirectoryPicker _directoryPicker;

        public ComponentViewModelFactory(IDirectoryPicker directoryPicker)
        {
            _directoryPicker = directoryPicker;
        }

        public IComponentViewModel Create(IComponent component)
        {
            return GetObjectViewModel((dynamic)component);
        }

        public IEnumerable<IComponentViewModel> CreateMany(IComponent component)
        {
            var entries = new List<IComponentViewModel>();
            if (component != null)
            {
                Fill(component, entries);
            }

            return entries;
        }

        private static IPropertyViewModel CreateEnumViewModel<TEnum>(IGenericProperty<TEnum> property)
        {
            return new PropertyViewModel<TEnum>(property, EqualityComparer<TEnum>.Default, new EnumPropertyConverter<TEnum>());
        }

        private void Fill(IComponent component, ICollection<IComponentViewModel> entries)
        {
            IComponentViewModel viewModel = GetObjectViewModel((dynamic)component);
            if (viewModel.Properties.Any())
            {
                entries.Add(viewModel);
            }

            foreach (var property in component.Properties.OfType<IComponent>())
            {
                Fill(property, entries);
            }
        }

        private ComponentViewModel<TEntry> GetObjectViewModel<TEntry>(Component<TEntry> component)
        {
            return component != null
                       ? new ComponentViewModel<TEntry>(component, GetProperties(component).ToArray())
                       : null;
        }

        private IEnumerable<IPropertyViewModel> GetProperties(IComponent component)
        {
            return component.Properties.Where(entry => entry.Configuration.IsAccessible)
                .Select(property => (IPropertyViewModel)GetViewModel((dynamic)property))
                .Where(viewModel => viewModel != null)
                .ToArray();
        }

        private IPropertyViewModel GetViewModel<TProperty>(IGenericProperty<TProperty> property)
        {
            if (property.Type == typeof(bool))
            {
                return new PropertyViewModel<bool>((IGenericProperty<bool>)property, EqualityComparer<bool>.Default, new BoolPropertyConverter());
            }

            if (property.Type == typeof(float))
            {
                return new PropertyViewModel<float>((IGenericProperty<float>)property, EqualityComparer<float>.Default, new FloatPropertyConverter());
            }

            if (property.Type == typeof(string))
            {
                if (property.Configuration.IsDirectoryPath)
                {
                    return new ReadonlyDirectoryPathViewModel((IGenericProperty<string>)property, _directoryPicker, EqualityComparer<string>.Default, new StringPropertyConverter());
                }

                return new PropertyViewModel<string>((IGenericProperty<string>)property, EqualityComparer<string>.Default, new StringPropertyConverter());
            }

            if (property.Type == typeof(int))
            {
                return new PropertyViewModel<int>((IGenericProperty<int>)property, EqualityComparer<int>.Default, new IntPropertyConverter());
            }

            if (property.Type == typeof(uint))
            {
                return new PropertyViewModel<uint>((IGenericProperty<uint>)property, EqualityComparer<uint>.Default, new UintPropertyConverter());
            }

            if (property.Type == typeof(ushort))
            {
                return new PropertyViewModel<ushort>((IGenericProperty<ushort>)property, EqualityComparer<ushort>.Default, new UshortPropertyConverter());
            }

            return property.Type.IsEnum
                       ? CreateEnumViewModel(property)
                       : null;
        }
    }
}