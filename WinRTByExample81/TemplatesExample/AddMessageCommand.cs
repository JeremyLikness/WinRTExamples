// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddMessageCommand.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The add message command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TemplatesExample
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The add message command.
    /// </summary>
    public class AddMessageCommand : ICommand
    {
        /// <summary>
        /// Delegate to add message action.
        /// </summary>
        private readonly Action addMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddMessageCommand"/> class.
        /// Constructor takes in the action to add the message
        /// </summary>
        /// <param name="addMessage">Action to add a message</param>
        public AddMessageCommand(Action addMessage)
        {
            this.addMessage = addMessage;
        }

        /// <summary>
        /// Can execute changed event
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Returns whether the command can execute
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        /// <returns>True if the command can execute</returns>
        public bool CanExecute(object parameter)
        {
            return this.addMessage != null; 
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        public void Execute(object parameter)
        {
            this.addMessage();
        }
    }
}
