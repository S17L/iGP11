using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

using iGP11.Library.Component;
using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.ViewModel;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.Framework
{
    public class ComponentConverter : MarkupExtension,
                                      IValueConverter
    {
        private static readonly IDictionary<WeakReference, IComponentViewModel> _dictionary = new Dictionary<WeakReference, IComponentViewModel>();
        private static readonly object _lock = new object();

        public FormType FormType { get; set; } = FormType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is IComponentViewModel)
            {
                return value;
            }

            var viewModelFactory = DependencyResolver.Current.Resolve<ComponentViewModelFactory>();
            if (viewModelFactory == null)
            {
                return null;
            }

            lock (_lock)
            {
                foreach (var pair in _dictionary.ToArray()
                    .Where(pair => pair.Key.Target == null))
                {
                    _dictionary.Remove(pair);
                }

                var holder = _dictionary.Select(pair => new
                    {
                        pair.Key.Target,
                        pair.Value
                    })
                    .FirstOrDefault(pair => (pair.Target != null) && ReferenceEquals(pair.Target, value));

                if (holder != null)
                {
                    return holder.Value;
                }

                var assembler = DependencyResolver.Current.Resolve<ComponentAssembler>();
                var assemblingContext = new AssemblingContext(FormType);
                var viewModel = viewModelFactory.Create(assembler.Assemble((dynamic)value, assemblingContext));
                _dictionary.Add(new WeakReference(value), viewModel);

                return viewModel;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}