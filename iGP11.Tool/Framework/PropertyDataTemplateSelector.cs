using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using iGP11.Library.Component.DataAnnotations;
using iGP11.Tool.ViewModel.PropertyEditor;

namespace iGP11.Tool.Framework
{
    public class PropertyDataTemplateSelector : DataTemplateSelector
    {
        [Order(1)]
        [PropertyDataType(typeof(bool))]
        public DataTemplate BoolDataTemplate { get; set; }

        [Order(6)]
        [DirectoryPathPropertyDataType]
        public DataTemplate DirectoryPathDataTemplate { get; set; }

        [Order(0)]
        [PropertyDataType(null)]
        public DataTemplate EnumDataTemplate { get; set; }

        [Order(2)]
        [PropertyDataType(typeof(float))]
        public DataTemplate FloatDataTemplate { get; set; }

        public DataTemplate GroupedByDataTemplate { get; set; }

        [Order(3)]
        [PropertyDataType(typeof(int))]
        public DataTemplate IntDataTemplate { get; set; }

        [Order(7)]
        [PropertyDataType(typeof(string))]
        public DataTemplate StringDataTemplate { get; set; }

        [Order(4)]
        [PropertyDataType(typeof(uint))]
        public DataTemplate UintDataTemplate { get; set; }

        [Order(5)]
        [PropertyDataType(typeof(ushort))]
        public DataTemplate UshortDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var groupedByViewModel = item as GroupedByViewModel;
            if (groupedByViewModel != null)
            {
                return GroupedByDataTemplate;
            }

            var propertyViewModel = item as IPropertyViewModel;
            if (propertyViewModel == null)
            {
                throw new ArgumentException($"item has invalid type: {item.GetType()}; acceptable type is: {typeof(IPropertyViewModel)}");
            }

            foreach (var property in from property in GetType()
                                         .GetProperties()
                                     let order = property.GetCustomAttributes(typeof(OrderAttribute), false)
                                         .Cast<OrderAttribute>()
                                         .FirstOrDefault()
                                     where order != null
                                     orderby order.Index ascending
                                     let attribute = property.GetCustomAttributes(typeof(PropertyDataTypeAttribute), false)
                                         .Cast<PropertyDataTypeAttribute>()
                                         .FirstOrDefault()
                                     where (attribute != null) && ((attribute.Type == propertyViewModel.Type) || ((attribute.Type == null) && propertyViewModel.Type.IsEnum)) && attribute.IsApplicable(propertyViewModel.Configuration)
                                     select property)
            {
                return (DataTemplate)property.GetValue(this);
            }

            throw new InvalidOperationException($"data template for: {item.GetType()} is not registered");
        }
    }
}