using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PlayToExample.Common
{
    /// <summary>
    /// An <see cref="IValueConverter"/> for setting items' visibility based on whether the incoming value is <c>null</c>.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullToVisibilityConverter"/> class.
        /// </summary>
        public NullToVisibilityConverter()
        {
            NullVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets the visibility value to use when the incoming value is <c>null</c>.
        /// </summary>
        /// <value>
        /// The visibility value to use when the incoming value is <c>null</c>.
        /// </value>
        public Visibility NullVisibility { get; set; }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return value == null
                ? (NullVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed)
                : (NullVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}