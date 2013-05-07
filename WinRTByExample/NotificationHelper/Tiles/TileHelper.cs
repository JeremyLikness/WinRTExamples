// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TileHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The tile helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Tiles
{
    using System;
    using System.Linq;
    using System.Text;

    using Windows.UI.Notifications;

    /// <summary>
    /// The tile helper.
    /// </summary>
    public static class TileHelper
    {
        /// <summary>
        /// The base tiles.
        /// </summary>
        private static BaseTile[] baseTiles;

        /// <summary>
        /// The get tiles.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseTile"/> array.
        /// </returns>
        public static BaseTile[] GetTiles()
        {
            return baseTiles
                   ?? (baseTiles =
                       Enum.GetNames(typeof(TileTemplateType))
                           .Select(tileType => (TileTemplateType)Enum.Parse(typeof(TileTemplateType), tileType))
                           .Select(tileTemplateType => new BaseTile(tileTemplateType))
                           .ToArray());
        }

        /// <summary>
        /// The get description.
        /// </summary>
        /// <param name="tile">
        /// The tile.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDescription(this BaseTile tile)
        {
            var sb = new StringBuilder();
            var type = tile.TileType.ToString();
            sb.Append(type).Append(" tile with ");
            if (tile.TextLines > 0)
            {
                sb.Append(tile.TextLines == 1 ? "one line of text" : string.Format("{0} lines of text", tile.TextLines));
            }

            if (tile.Images > 0)
            {
                if (tile.TextLines > 0)
                {
                    sb.Append(" and");
                }
                
                if (tile.TemplateType.Contains("Peek"))
                {
                    sb.Append(" peek image with");
                }
                 
                sb.Append(tile.Images == 1 ? " one image" : string.Format(" {0} images", tile.Images));
            }

            sb.Append(".");
            return sb.ToString();
        }

        /// <summary>
        /// Initializes static members of the <see cref="TileHelper"/> class.
        /// </summary>
        /// <param name="tileTemplateType">
        /// The tile Template Type.
        /// </param>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public static BaseTile GetTile(this TileTemplateType tileTemplateType)
        {
            return new BaseTile(tileTemplateType);                
        }
    }
}