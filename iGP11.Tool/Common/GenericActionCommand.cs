using System;

namespace iGP11.Tool.Common
{
    public class GenericActionCommand<TObject> : IActionCommand
    {
        private readonly Action<TObject> _action;
        private readonly Func<bool> _canExecute;

        public GenericActionCommand(Action<TObject> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _action((TObject)parameter);
        }

        public void Rebind()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}