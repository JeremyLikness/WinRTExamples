// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TileItem.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The tile item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.DataModel
{
    using WinRTByExample.NotificationHelper.Tiles;

    /// <summary>
    /// The tile item.
    /// </summary>
    public class TileItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileItem"/> class.
        /// </summary>
        /// <param name="tile">
        /// The tile.
        /// </param>
        public TileItem(BaseTile tile)
        {
            this.Id = tile.TemplateType;
            this.Tile = tile;
            this.Xml = System.Xml.Linq.XDocument.Parse(tile.ToString()).ToString();            
            this.Description = tile.GetDescription();
        }

        /// <summary>
        /// Gets the base tile
        /// </summary>
        public BaseTile Tile { get; private set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the xml.
        /// </summary>
        public string Xml { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }
    }
}
