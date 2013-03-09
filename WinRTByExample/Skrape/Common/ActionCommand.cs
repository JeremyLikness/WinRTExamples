// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionCommand.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The action command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Common
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The action command.
    /// </summary>
    public class ActionCommand : ICommand 
    {
        /// <summary>
        /// The action.
        /// </summary>
        private readonly Action action;

        /// <summary>
        /// The condition.
        /// </summary>
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        public ActionCommand()
        {
            this.action = delegate { };
            this.condition = () => true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public ActionCommand(Action action)
        {
            this.action = action;
            this.condition = () => true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public ActionCommand(Action action, Func<bool> condition)
        {
            this.action = action;
            this.condition = condition;
        }


        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// The raise execute changed method.
        /// </summary>
        public void RaiseExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.condition();
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.action();
        }
    }
}
