// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTile.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base tile.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Tiles
{
    using System;

    using Windows.UI.Notifications;

    using WinRTByExample.NotificationHelper.Common;

    /// <summary>
    /// The base tile.
    /// </summary>
    public class BaseTile : BaseNotification<BaseTile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTile"/> class.
        /// </summary>
        /// <param name="templateType">
        /// The template Type.
        /// </param>
        public BaseTile(TileTemplateType templateType) : base(TileUpdateManager.GetTemplateContent(templateType), templateType.ToString())
        {
            this.Type = templateType;
            this.TileType = this.TemplateType.StartsWith("TileSquare") ? TileTypes.Square : TileTypes.Wide;            
        }

        /// <summary>
        /// Gets the tile type.
        /// </summary>
        public TileTypes TileType { get; private set; }

        /// <summary>
        /// Gets the tile template type
        /// </summary>
        public TileTemplateType Type { get; private set; }

        /// <summary>
        /// Gets or sets the tag for the tile
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets the expiration setter.
        /// </summary>
        protected override Action<object, DateTime> ExpirationSetter
        {
            get
            {
                return (obj, expires) => ((TileNotification)obj).ExpirationTime = expires;
            }
        }

        /// <summary>
        /// The send method
        /// </summary>
        public void Set()
        {
            var tile = new TileNotification(this.Xml);            
            this.SetExpiration(tile);

            if (!string.IsNullOrWhiteSpace(this.Tag))
            {
                tile.Tag = this.Tag;
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
        }

        /// <summary>
        /// With notifications - iterate multiple tiles
        /// </summary>
        /// <returns>The base tile</returns>
        public BaseTile WithNotifications()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            return this;
        }

        /// <summary>
        /// Without notifications - turn off notifications
        /// </summary>
        /// <returns>The base tile</returns>
        public BaseTile WithoutNotifications()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(false);
            return this;
        }

        /// <summary>
        /// With notifications - iterate multiple tiles
        /// </summary>
        /// <param name="secondaryId">
        /// The secondary Id.
        /// </param>
        /// <returns>
        /// The base tile
        /// </returns>
        public BaseTile WithSecondaryNotifications(string secondaryId)
        {
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryId).EnableNotificationQueue(true);
            return this;
        }

        /// <summary>
        /// Without notifications - turn off notifications
        /// </summary>
        /// <param name="secondaryId">
        /// The secondary Id.
        /// </param>
        /// <returns>
        /// The base tile
        /// </returns>
        public BaseTile WithoutSecondaryNotifications(string secondaryId)
        {
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryId).EnableNotificationQueue(false);
            return this;
        }

        /// <summary>
        /// Set secondary tile
        /// </summary>
        /// <param name="secondaryId">Identifier of the secondary tile</param>
        public void SetSecondary(string secondaryId)
        {
            var tile = new TileNotification(this.Xml);
            this.SetExpiration(tile);

            if (!string.IsNullOrWhiteSpace(this.Tag))
            {
                tile.Tag = this.Tag;
            }

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryId).Update(tile);
        }

        /// <summary>
        /// The with no branding.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile WithNoBranding()
        {
            var visual = this.Xml.GetElementsByTagName("visual")[0];
            var branding = this.Xml.CreateAttribute("branding");
            branding.NodeValue = "none";
            visual.Attributes.SetNamedItem(branding);
            return this;
        }

        /// <summary>
        /// The with logo branding.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile WithLogoBranding()
        {
            var visual = this.Xml.GetElementsByTagName("visual")[0];
            var branding = this.Xml.CreateAttribute("branding");
            branding.NodeValue = "logo";
            visual.Attributes.SetNamedItem(branding);
            return this;
        }

        /// <summary>
        /// The with name branding.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile WithNameBranding()
        {
            var visual = this.Xml.GetElementsByTagName("visual")[0];
            var branding = this.Xml.CreateAttribute("branding");
            branding.NodeValue = "logo";
            visual.Attributes.SetNamedItem(branding);
            return this;
        }

        /// <summary>
        /// Set a tag for the tile
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The tile</returns>
        public BaseTile WithTag(string tag)
        {
            this.Tag = tag;
            return this;
        }

        /// <summary>
        /// Merges two tiles - i.e. when you want a square tile and a wide tile 
        /// </summary>
        /// <param name="otherTile">The other tile to include</param>
        /// <returns>This tile with the other tile merged</returns>
        public BaseTile WithTile(BaseTile otherTile)
        {
            var otherBinding = this.Xml.ImportNode(otherTile.Xml.GetElementsByTagName("visual")[0].LastChild, true);
            this.Xml.GetElementsByTagName("visual")[0].AppendChild(otherBinding);
            return this;
        }
    }
}
