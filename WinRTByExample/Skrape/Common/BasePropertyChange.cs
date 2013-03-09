// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePropertyChange.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base property change.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Common
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The base property change.
    /// </summary>
    public abstract class BasePropertyChange : INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// The on property changed method
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));            
        }
    }
}
