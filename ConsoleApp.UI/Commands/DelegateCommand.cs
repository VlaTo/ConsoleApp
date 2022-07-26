using System;
using System.Windows.Input;

namespace ConsoleApp.UI.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;
        private readonly Func<bool> condition;
        private bool? canExecute;
        
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action, Func<bool> condition = null)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.action = action;
            this.condition = condition ?? Always;
            
            canExecute = null;
        }

        public bool CanExecute(object _)
        {
            var result = condition.Invoke();

            if (false == canExecute.HasValue || canExecute != result)
            {
                canExecute = result;
                RaiseCanExecuteChanged(EventArgs.Empty);
            }

            return result;
        }

        public void Execute(object parameter)
        {
            if (false == canExecute.HasValue)
            {
                CanExecute(parameter);
            }

            if (true == canExecute)
            {
                action.Invoke();
            }
        }

        private void RaiseCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private static bool Always() => true;
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> action;
        private readonly Predicate<T> condition;
        private bool? canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> action, Predicate<T> condition = null)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.action = action;
            this.condition = condition ?? Always;

            canExecute = null;
        }

        public bool CanExecute(object parameter)
        {
            var result = condition.Invoke((T)parameter);

            if (false == canExecute.HasValue || canExecute != result)
            {
                canExecute = result;
                RaiseCanExecuteChanged(EventArgs.Empty);
            }

            return result;
        }

        public void Execute(object parameter)
        {
            if (false == canExecute.HasValue)
            {
                CanExecute(parameter);
            }

            if (true == canExecute)
            {
                action.Invoke((T)parameter);
            }
        }

        private void RaiseCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private static bool Always(T _) => true;
    }
}