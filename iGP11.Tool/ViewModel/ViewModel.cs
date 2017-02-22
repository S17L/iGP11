using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using iGP11.Library;

namespace iGP11.Tool.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> expression)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(expression.GetMemberName()));
        }
    }
}