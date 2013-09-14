// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherCommand.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The weather command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The weather command.
    /// </summary>
    public class WeatherCommand : ICommand 
    {
        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Gets or sets the execution action
        /// </summary>
        public Action<string> ExecuteAction { get; set; }

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
            if (this.ExecuteAction == null)
            {
                return false;
            }

            var value = parameter as string;
            
            if (value == null)
            {
                return false;
            }

            if (value.Length != 5)
            {
                return false;
            }

            int zipCode;

            return int.TryParse(value, out zipCode);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.ExecuteAction(parameter.ToString());
            }
        }
        
        /// <summary>
        /// The on can execute change.
        /// </summary>
        public void OnCanExecuteChange()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}