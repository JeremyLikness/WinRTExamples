// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BadgeItem.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The badge item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer.DataModel
{
    using WinRTByExample.NotificationHelper.Badges;

    /// <summary>
    /// The tile item.
    /// </summary>
    public class BadgeItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadgeItem"/> class.
        /// </summary>
        /// <param name="badge">
        /// The badge.
        /// </param>
        public BadgeItem(BaseBadge badge)
        {
            this.Id = badge.TemplateType;
            this.Badge = badge;
            this.Xml = badge.ToString();
            this.Description = badge.GetDescription();
        }

        /// <summary>
        /// Gets the base badge
        /// </summary>
        public BaseBadge Badge { get; private set; }

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
