﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PercentageConverter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The percentage converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataBindingExample
{
    using System;

    using Windows.UI.Xaml.Data;

    /// <summary>
    /// The percentage converter.
    /// </summary>
    public class PercentageConverter : IValueConverter 
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return ((double)value) / 100.0;
            }

            return value;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">Not implemented
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
