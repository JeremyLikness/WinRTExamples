// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The dependency model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataBindingExample
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using Windows.UI.Xaml;

    /// <summary>
    /// The dependency model.
    /// </summary>
    public class DependencyModel : DependencyObject
    {
        /// <summary>
        /// The percentage property.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", 
            Justification = "Reviewed. Suppression is OK here for dependency property.")]
        public static DependencyProperty PercentageProperty = DependencyProperty.Register(
            "Percentage", typeof(double), typeof(DependencyModel), new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public double Percentage
        {
            get
            {
                return (double)this.GetValue(PercentageProperty);
            }

            set
            {
                this.SetValue(PercentageProperty, value);
            }
        }

        /// <summary>
        /// The on property change.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Dependency property changed from {0} to {1}", e.OldValue, e.NewValue);
        }
    }
}
