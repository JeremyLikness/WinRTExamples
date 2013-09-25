// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TileToSizeConverter.cs" company="Jeremy Likness">
//   Converter for tile size
// </copyright>
// <summary>
//   The tile to example converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.Common
{
    using System;

    using DataModel;

    using Windows.UI.Xaml.Data;

    using WinRTByExample.NotificationHelper.Tiles;

    /// <summary>
    /// The tile to example converter.
    /// </summary>
    public class TileToSizeConverter : IValueConverter
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value (a tile).
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter (w if getting width).
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The width or height of the tile.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double width, height;

            var tile = value as TileItem;

            if (tile == null || tile.Tile.TileType.Equals(TileTypes.Square))
            {
                width = 150.0;
                height = 150.0;
            }
            else if (tile.Tile.TileType.Equals(TileTypes.Wide))
            {
                width = 310.0;
                height = 150.0;
            }
            else
            {
                width = 310.0;
                height = 310.0;
            }

            if (tile != null && tile.Tile.TemplateType.Contains("Peek"))
            {
                height *= 2.0;
            }
    
            if (parameter != null && parameter.ToString().Equals("w"))
            {
                return width;
            }

            return height;
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