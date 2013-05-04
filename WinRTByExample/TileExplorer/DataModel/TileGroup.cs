// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TileGroup.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The tile group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.DataModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The tile group.
    /// </summary>
    public class TileGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileGroup"/> class.
        /// </summary>
        public TileGroup()
        {
            this.Items = new List<TileItem>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public List<TileItem> Items { get; set; } 
    }
}
