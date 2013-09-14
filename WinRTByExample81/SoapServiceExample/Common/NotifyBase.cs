// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyBase.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The notify base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Common
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The notify base.
    /// </summary>
    public class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// The on property changed method.
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
