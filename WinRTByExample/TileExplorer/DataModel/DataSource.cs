// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.DataModel
{
    using System.Collections.Generic;
    using System.Linq;

    using TileExplorer.Tiles;

    /// <summary>
    /// The data source.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// The groups.
        /// </summary>
        private readonly List<TileGroup> groups = new List<TileGroup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        public DataSource()
        {
            var tileList = TileHelper.GetTiles();
            var squares = new TileGroup { Name = "Square" };
            foreach (var tile in tileList.Where(tile => tile.TileType.Equals(TileTypes.Square)))
            {
                squares.Items.Add(new TileItem(tile));
            }

            var wides = new TileGroup { Name = "Wide" };
            foreach (var tile in tileList.Where(tile => tile.TileType.Equals(TileTypes.Wide)))
            {
                wides.Items.Add(new TileItem(tile));
            }

            this.groups.Add(squares);
            this.groups.Add(wides);
        }
    }
}