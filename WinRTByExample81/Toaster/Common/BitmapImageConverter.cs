// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapImageConverter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The bitmap image converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Toaster.Common
{
    using System;

    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The bitmap image converter.
    /// </summary>
    public class BitmapImageConverter : IValueConverter 
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
            if (value is string)
            {
                var path = string.Format("ms-appx:///Assets/{0}", value);
                var bitmapImage = new BitmapImage(new Uri(path, UriKind.Absolute));
                return bitmapImage;
            }

            return null;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="language">The language</param>
        /// <returns>This is not implemented</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
