// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Windows.UI.Notifications;

namespace TileExplorer.DataModel
{
    using System.Collections.Generic;
    using System.Linq;

    using WinRTByExample.NotificationHelper.Badges;
    using WinRTByExample.NotificationHelper.Tiles;

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
        /// The badges
        /// </summary>
        private readonly List<BadgeItem> badges = new List<BadgeItem>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        public DataSource()
        {
            var tileList = TileHelper.GetTiles();

            var squares = new TileGroup { Name = TileTypes.Square.ToString() };
            foreach (var tile in tileList.Where(tile => tile.TileType.Equals(TileTypes.Square)))
            {
                squares.Items.Add(new TileItem(tile));
            }

            var wides = new TileGroup { Name = TileTypes.Wide.ToString() };
            foreach (var tile in tileList.Where(tile => tile.TileType.Equals(TileTypes.Wide)))
            {
                wides.Items.Add(new TileItem(tile));
            }

            var larges = new TileGroup {Name = TileTypes.Large.ToString()};
            foreach (var tile in tileList.Where(tile => tile.TileType.Equals(TileTypes.Large)))
            {
                larges.Items.Add(new TileItem(tile));
            }

            this.groups.Add(squares);
            this.groups.Add(wides);
            this.groups.Add(larges);

            var badgeList = BadgeHelper.GetBadges();
            foreach (var badge in badgeList
                .Where(badge => !badge.GlyphType.Equals(BadgeGlyphTypes.None) 
                && !badge.GlyphType.Equals(BadgeGlyphTypes.Numeric)))
            {
                this.badges.Add(new BadgeItem(badge));
            }
        }

        /// <summary>
        /// Gets the badges
        /// </summary>
        public IEnumerable<BadgeItem> Badges
        {
            get
            {
                return this.badges;
            }
        }

        /// <summary>
        /// Gets the all groups.
        /// </summary>
        public IEnumerable<TileGroup> AllGroups
        {
            get
            {
                return this.groups;
            }
        }

        /// <summary>
        /// The get groups.
        /// </summary>
        /// <returns>
        /// The list of groups
        /// </returns>
        public IEnumerable<TileGroup> GetGroups()
        {
            return this.groups;
        }

        /// <summary>
        /// Get specific group
        /// </summary>
        /// <param name="groupName">The type of the group</param>
        /// <returns>The group</returns>
        public TileGroup GetGroup(string groupName)
        {
            return this.groups.FirstOrDefault(g => g.Name.Equals(groupName));
        }

        /// <summary>
        /// Get the group an item belongs to
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <returns>The corresponding <see cref="TileGroup"/></returns>
        public TileGroup GetGroupForItem(string id)
        {
            return this.groups.SelectMany(g => g.Items, (g, t) => new { group = g, tile = t })
                        .Where(groupTile => groupTile.tile.Id.Equals(id))
                        .Select(groupTile => groupTile.group).FirstOrDefault();
        }

        /// <summary>
        /// The get tile.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TileItem"/>.
        /// </returns>
        public TileItem GetTile(string id)
        {
            return this.groups.SelectMany(g => g.Items, (g, t) => new { group = g, tile = t })
                        .Where(groupTile => groupTile.tile.Id.Equals(id))
                        .Select(groupTile => groupTile.tile).FirstOrDefault();
        }
    }
}