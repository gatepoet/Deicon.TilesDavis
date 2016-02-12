using System;
using System.Windows.Input;

namespace TilesDavis.Wpf
{
    public class ActionCommand : ICommand
    {
        private Action action;
        private Predicate<object> canExecute;

        public ActionCommand(Action action, Predicate<object> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            var result = canExecute?.Invoke(parameter);
            return result.HasValue
                ? result.Value
                : true;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
    }
    public class ActionCommand<T> : ICommand
        where T : class
    {
        private Action<T> action;
        private Predicate<T> canExecute;

        public ActionCommand(Action<T> action, Predicate<T> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            var result = canExecute?.Invoke(parameter as T);
            return result.HasValue && result.Value;
        }

        public void Execute(object parameter)
        {
            action.Invoke(parameter as T);
        }

    }
}
    