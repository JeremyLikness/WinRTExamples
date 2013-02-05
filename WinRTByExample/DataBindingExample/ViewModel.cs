// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataBindingExample
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using DataBindingExample.Annotations;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged 
    {
        /// <summary>
        /// The percentage.
        /// </summary>
        private double percentage;

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public double Percentage
        {
            get
            {
                return this.percentage;
            }

            set
            {
                if (value.Equals(this.percentage))
                {
                    return;
                }

                Debug.WriteLine("ViewModel property changed from {0} to {1}", this.percentage, value);

                this.percentage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}