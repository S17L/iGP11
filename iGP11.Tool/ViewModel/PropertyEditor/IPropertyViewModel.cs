using System;
using System.Collections.Generic;
using System.ComponentModel;

using iGP11.Library.Component;

namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public delegate void PropertyViewModelChangedEventHandler(IPropertyViewModel propertyViewModel, EventArgs eventArgs);

    public interface IPropertyViewModel : INotifyPropertyChanged,
                                          INotifyDataErrorInfo
    {
        event PropertyViewModelChangedEventHandler Changed;

        IPropertyConfiguration Configuration { get; }

        string Name { get; }

        Type Type { get; }

        IEnumerable<string> GetDataValidationErrors();

        void Rebind();
    }
}