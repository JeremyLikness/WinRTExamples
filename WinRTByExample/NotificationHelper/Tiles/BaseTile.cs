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

    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;

    /// <summary>
    /// The base tile.
    /// </summary>
    public class BaseTile 
    {
        /// <summary>
        /// The xml.
        /// </summary>
        private readonly XmlDocument xml;

        /// <summary>
        /// Index of text 
        /// </summary>
        private uint textIndex;

        /// <summary>
        /// Index of image
        /// </summary>
        private uint imageIndex; 

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTile"/> class.
        /// </summary>
        /// <param name="templateType">
        /// The template Type.
        /// </param>
        public BaseTile(TileTemplateType templateType)
        {
            this.Type = templateType;
            this.TemplateType = templateType.ToString();
            this.TileType = this.TemplateType.StartsWith("TileSquare") ? TileTypes.Square : TileTypes.Wide;
            this.xml = TileUpdateManager.GetTemplateContent(templateType);
            this.TextLines = this.xml.GetElementsByTagName("text").Count;
            this.Images = this.xml.GetElementsByTagName("image").Count;
        }

        /// <summary>
        /// Gets or sets the template type.
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// Gets the tile type.
        /// </summary>
        public TileTypes TileType { get; private set; }

        /// <summary>
        /// Gets the tile template type
        /// </summary>
        public TileTemplateType Type { get; private set; }

        /// <summary>
        /// Gets lines of text the tile supports
        /// </summary>
        public int TextLines { get; private set; }

        /// <summary>
        /// Gets total images the tile supports
        /// </summary>
        public int Images { get; private set; }
        
        /// <summary>
        /// Tags the tile
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The send method
        /// </summary>
        public void Set()
        {
            var tile = new TileNotification(this.xml) { Tag = this.Tag };
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
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(secondaryId).Update(new TileNotification(this.xml));
        }

        /// <summary>
        /// To string method
        /// </summary>
        /// <returns>The xml for the tile</returns>
        public override string ToString()
        {
            return this.xml.GetXml();
        }

        /// <summary>
        /// The with no branding.
        /// </summary>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile WithNoBranding()
        {
            var visual = this.xml.GetElementsByTagName("visual")[0];
            var branding = this.xml.CreateAttribute("branding");
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
            var visual = this.xml.GetElementsByTagName("visual")[0];
            var branding = this.xml.CreateAttribute("branding");
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
            var visual = this.xml.GetElementsByTagName("visual")[0];
            var branding = this.xml.CreateAttribute("branding");
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
            var otherBinding = this.xml.ImportNode(otherTile.xml.GetElementsByTagName("visual")[0].LastChild, true);
            this.xml.GetElementsByTagName("visual")[0].AppendChild(otherBinding);
            return this;
        }

        /// <summary>
        /// Add a text element 
        /// </summary>
        /// <param name="text">
        /// The text to add
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile AddText(string text, uint id = 0)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("text");
            }

            if (id == 0)
            {
                id = this.textIndex++;
            }

            if (id >= this.TextLines)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            var elements = this.xml.GetElementsByTagName("text");

            var node = elements.Item(id); 

            if (node != null)
            {
                node.AppendChild(this.xml.CreateTextNode(text));
            }

            return this;
        }

        /// <summary>
        /// The add image.
        /// </summary>
        /// <param name="imageUri">
        /// The image uri.
        /// </param>
        /// <param name="alt">
        /// The alt.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="BaseTile"/>.
        /// </returns>
        public BaseTile AddImage(string imageUri, string alt, uint id = 0)
        {
            if (string.IsNullOrWhiteSpace(imageUri))
            {
                throw new ArgumentException("imageUri");
            }

            if (id == 0)
            {
                id = this.imageIndex++;
            }

            if (id >= this.Images)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            var elements = this.xml.GetElementsByTagName("image");

            var node = (XmlElement)elements.Item(id);

            if (node == null)
            {
                return this;
            }

            node.SetAttribute("src", imageUri);

            if (!string.IsNullOrWhiteSpace(alt))
            {
                node.SetAttribute("alt", alt);
            }

            return this;
        }        
    }
}
