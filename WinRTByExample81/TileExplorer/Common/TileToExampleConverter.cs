// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TileToExampleConverter.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The tile to example converter - takes a tile and maps to the example image
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.Common
{
    using System;

    using DataModel;

    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The tile to example converter.
    /// </summary>
    public class TileToExampleConverter : IValueConverter 
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
            var uri = new Uri("ms-appx:///Assets/DarkGray.png");
            var tile = value as TileItem;
            if (tile != null)
            {
                uri = new Uri(string.Format("ms-appx:///Examples/{0}.png", tile.Tile.TemplateType));
            }
            else
            {
                var badge = value as BadgeItem;
                if (badge != null)
                {
                    uri = new Uri(string.Format("ms-appx:///Examples/{0}.png", badge.Badge.TemplateType));
                }
            }

            return new BitmapImage(uri);
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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}