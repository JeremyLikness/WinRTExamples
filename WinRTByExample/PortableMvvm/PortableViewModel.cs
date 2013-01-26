// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortableViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the PortableViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PortableMvvm
{
    using System.ComponentModel;
    using System.Windows.Input;

    using PortableMvvm.Annotations;

    /// <summary>
    /// The portable view model.
    /// </summary>
    public class PortableViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The tap text.
        /// </summary>
        private string tapText;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortableViewModel"/> class.
        /// </summary>
        public PortableViewModel()
        {
            this.TapCommand = new RunOnceCommand(this.OnTapped);
            this.TapText = "Tap or Click Me.";
        }

            /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the tap command.
        /// </summary>
        public ICommand TapCommand { get; private set; }

        /// <summary>
        /// Gets or sets the tap text.
        /// </summary>
        public string TapText
        {
            get
            {
                return this.tapText;
            }
            
            set
            {
                if (value == this.tapText)
                {
                    return;
                }

                this.tapText = value;
                this.OnPropertyChanged("TapText");
            }
        }

        /// <summary>
        /// The on property changed method.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The on tapped.
        /// </summary>
        private void OnTapped()
        {
            this.TapText = "Disabled.";
        }
    }
}