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

    using TileExplorer.DataModel;

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
            var widthOrHeight = 150.0;
            var tile = value as TileItem;
                
            if (parameter != null && parameter.ToString().Equals("w"))
            {
                if (tile != null && tile.Tile.TileType.Equals(TileTypes.Wide))
                {
                    widthOrHeight = 300.0;
                }
            }

            if (parameter != null && parameter.ToString().Equals("h"))
            {
                if (tile != null && tile.Tile.TemplateType.Contains("Peek"))
                {
                    widthOrHeight = 300.0;
                }
            }

            return widthOrHeight;
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