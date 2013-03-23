// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThumbnailConverter.cs" company="Microsoft">
//   From http://code.msdn.microsoft.com/windowsapps/Data-source-adapter-sample-3d32e535/sourcecode?fileId=52069&pathId=1722905476
// </copyright>
// <summary>
//   The thumbnail converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QueryMusicLibrary
{
    using System;

    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The thumbnail converter.
    /// </summary>
    public class ThumbnailConverter : IValueConverter
    {
        /// <summary>
        /// The convert method.
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
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                var thumbnailStream = (IRandomAccessStream)value;
                var image = new BitmapImage();
                image.SetSource(thumbnailStream);

                return image;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// The convert back method - not implemented.
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
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown because it is not implemented
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    } 
}
