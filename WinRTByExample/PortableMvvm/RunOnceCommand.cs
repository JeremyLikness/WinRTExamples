// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunOnceCommand.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The run once command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortableMvvm
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The run once command.
    /// </summary>
    public class RunOnceCommand : ICommand
    {
        /// <summary>
        /// The _thing to do.
        /// </summary>
        private readonly Action thingToDo 
            = delegate { };

        /// <summary>
        /// The _already ran.
        /// </summary>
        private bool alreadyRan;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="RunOnceCommand"/> class.
        /// </summary>
        /// <param name="thingToDo">
        /// The thing to do.
        /// </param>
        public RunOnceCommand(Action thingToDo)
        {
            this.thingToDo = thingToDo;
        }
       
        /// <summary>
        /// The can execute changed event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// The can execute check.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// True <see cref="bool"/> if it hasn't run yet.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return !this.alreadyRan;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (this.alreadyRan)
            {
                return;
            }

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
