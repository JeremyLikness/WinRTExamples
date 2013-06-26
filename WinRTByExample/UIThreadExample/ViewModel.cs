// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the ViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UIThreadExample
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Windows.UI.Core;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : ICommand, INotifyPropertyChanged
    {
        /// <summary>
        /// UI dispatcher
        /// </summary>
        private readonly CoreDispatcher dispatcher;

        /// <summary>
        /// The date/time 
        /// </summary>
        private DateTime dateTime = DateTime.Now; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };       

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return this.dateTime;
            }

            set
            {
                this.dateTime = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Time"));
            }
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
            var cmd = parameter as string;
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return false;
            }

            return cmd == "correct" || cmd == "incorrect";
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
                Task.Run(async () => await this.AsyncCommand((string)parameter));
            }
        }

        /// <summary>
        /// The async command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task AsyncCommand(string command)
        {
            await Task.Delay(500);
            if (command == "correct")
            {
                await this.dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, 
                    () => this.Time = DateTime.Now);
            }
            else
            {
                this.Time = DateTime.Now;
            }
        }
    }
}