// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseBadge.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base tile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Badges
{
    using System;

    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;

    /// <summary>
    /// The base tile.
    /// </summary>
    public class BaseBadge
    {
        /// <summary>
        /// The xml.
        /// </summary>
        private readonly XmlDocument xml;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinRTByExample.NotificationHelper.Badges.BaseBadge"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public BaseBadge(BadgeGlyphTypes type)
        {
            this.Type = BadgeTemplateType.BadgeGlyph;
            this.GlyphType = type;
            this.TemplateType = type.ToString();
            this.xml = BadgeUpdateManager.GetTemplateContent(this.Type);
            var badgeToLower = string.Format(
                "{0}{1}", this.TemplateType.Substring(0, 1).ToLower(), this.TemplateType.Substring(1));
            var xmlElement = (XmlElement)this.xml.GetElementsByTagName("badge").Item(0);
            if (xmlElement != null)
            {
                xmlElement.SetAttribute("value", badgeToLower);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseBadge"/> class. 
        /// </summary>
        /// <param name="value">The initial value for the badge
        /// </param>
        public BaseBadge(int value)
        {
            if (value < 0 || value > 99)
            {
                throw new ArgumentException("value");
            }

            this.NumericValue = value;
            this.Type = BadgeTemplateType.BadgeNumber;
            this.GlyphType = BadgeGlyphTypes.Numeric;
            this.TemplateType = this.GlyphType.ToString();
            this.xml = BadgeUpdateManager.GetTemplateContent(this.Type);
            var xmlElement = (XmlElement)this.xml.GetElementsByTagName("badge").Item(0);
            if (xmlElement != null)
            {
                xmlElement.SetAttribute("value", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the template type.
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// Gets the glyph type
        /// </summary>
        public BadgeGlyphTypes GlyphType { get; private set; }

        /// <summary>
        /// Gets the badge type.
        /// </summary>
        public BadgeTemplateType Type { get; private set; }

        /// <summary>
        /// Gets the numeric value of the badge
        /// </summary>
        public int NumericValue { get; private set; }

        /// <summary>
        /// The send method
        /// </summary>
        public void Set()
        {
            var badge = new BadgeNotification(this.xml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        /// <summary>
        /// Set secondary tile
        /// </summary>
        /// <param name="secondaryId">Identifier of the secondary tile</param>
        public void SetSecondary(string secondaryId)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(secondaryId).Update(new BadgeNotification(this.xml));
        }

        /// <summary>
        /// To string method
        /// </summary>
        /// <returns>The xml for the tile</returns>
        public override string ToString()
        {
            return this.xml.GetXml();
        }
    }
}
