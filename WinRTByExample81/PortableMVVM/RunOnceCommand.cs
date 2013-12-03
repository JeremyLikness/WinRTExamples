namespace PortableMVVM
{
    using System;
    using System.Windows.Input;

    public class RunOnceCommand : ICommand
    {
        private readonly Action thingToDo = delegate { };

        private bool alreadyRan;

        public RunOnceCommand(Action thingToDo)
        {
            this.thingToDo = thingToDo;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return !this.alreadyRan;
        }

        public void Execute(object parameter)
        {
            this.thingToDo();
            this.alreadyRan = true;

            var handler = this.CanExecuteChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
