namespace AdvancedHttpExample.Data
{
    using System;
    using System.Windows.Input;

    public class ActionCommand : ICommand
    {
        private readonly Action<object> action = obj => { };

        private readonly Predicate<object> permission = obj => true;

        public ActionCommand()
        {

        }

        public ActionCommand(Action<object> action)
        {
            this.action = action;
        }

        public ActionCommand(Action<object> action, Predicate<object> permission)
        {
            this.action = action;
            this.permission = permission;
        }

        public bool CanExecute(object parameter)
        {
            return this.permission(parameter);
        }

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.action(parameter);
            }
        }

        public void OnCanExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged = delegate { };
    }
}