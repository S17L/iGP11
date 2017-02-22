using System;
using System.Collections.Generic;

using iGP11.Tool.Model;

namespace iGP11.Tool.ViewModel.PropertyEditor
{
    public delegate void StateChangedEventHandler(ViewModel viewModel, EventArgs eventArgs);

    public delegate void ValidationTriggeredEventHandler(ViewModel viewModel, ValidationResultEventArgs eventArgs);

    public interface IComponentViewModel
    {
        event StateChangedEventHandler Changed;

        event ValidationTriggeredEventHandler ValidationTriggered;

        int ErrorCount { get; }

        bool HasErrors { get; }

        string LongDescription { get; }

        string Name { get; }

        IEnumerable<IPropertyViewModel> Properties { get; }

        string ShortDescription { get; }

        void Rebind();
    }
}