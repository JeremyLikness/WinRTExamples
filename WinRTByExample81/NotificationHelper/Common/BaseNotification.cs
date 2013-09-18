// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseNotification.cs" company="Jeremy Likness">
//   The base notification class
// </copyright>
// <summary>
//   Defines the BaseNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Common
{
    using System;

    using Windows.Data.Xml.Dom;

    /// <summary>
    /// The base notification.
    /// </summary>
    /// <typeparam name="T">The type of the derived class
    /// </typeparam>
    public abstract class BaseNotification<T> where T : BaseNotification<T>
    {
        /// <summary>
        /// The xml.
        /// </summary>
        protected readonly XmlDocument Xml;

        /// <summary>
        /// Index of text 
        /// </summary>
        private uint textIndex;

        /// <summary>
        /// Index of image
        /// </summary>
        private uint imageIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNotification{T}"/> class.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        protected BaseNotification(XmlDocument xml, string templateType)
        {
            this.Xml = xml;
            this.TextLines = this.Xml.GetElementsByTagName("text").Count;
            this.Images = this.Xml.GetElementsByTagName("image").Count;
            this.TemplateType = templateType;
        }

        /// <summary>
        /// Gets lines of text the tile supports
        /// </summary>
        public int TextLines { get; private set; }

        /// <summary>
        /// Gets total images the tile supports
        /// </summary>
        public int Images { get; private set; }

        /// <summary>
        /// Gets the template type
        /// </summary>
        public string TemplateType { get; private set; }

        /// <summary>
        /// Gets the expiration time of the toast
        /// </summary>
        public DateTime? Expiration { get; private set; }

        /// <summary>
        /// Gets the function to set the expiration
        /// </summary>
        protected abstract Action<object, DateTime> ExpirationSetter { get; } 

        /// <summary>
        /// The with expiration using a <see cref="DateTime"/>
        /// </summary>
        /// <param name="expires">
        /// The expires.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T WithExpiration(DateTime expires)
        {
            this.Expiration = expires;
            return (T)this;
        }

        /// <summary>
        /// The with expiration using a <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="expires">
        /// The time span to expire within
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T WithExpiration(TimeSpan expires)
        {
            this.Expiration = DateTime.Now.Add(expires);
            return (T)this;
        }

        /// <summary>
        /// To string method
        /// </summary>
        /// <returns>The xml for the tile</returns>
        public override string ToString()
        {
            return this.Xml.GetXml();
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
        /// Instance of self
        /// </returns>
        public T AddText(string text, uint id = 0)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("text");
            }

            if (id == 0)
            {
                id = this.textIndex++;
            }

            if (id > this.TextLines)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            var elements = this.Xml.GetElementsByTagName("text");

            var node = elements.Item(id);

            if (node != null)
            {
                node.AppendChild(this.Xml.CreateTextNode(text));
            }

            return (T)this;
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
        /// The instance of self
        /// </returns>
        public T AddImage(string imageUri, string alt, uint id = 0)
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

            var elements = this.Xml.GetElementsByTagName("image");

            var node = (XmlElement)elements.Item(id);

            if (node == null)
            {
                return (T)this;
            }

            node.SetAttribute("src", imageUri);

            if (!string.IsNullOrWhiteSpace(alt))
            {
                node.SetAttribute("alt", alt);
            }

            return (T)this;
        }

        /// <summary>
        /// Set the expiration if set
        /// </summary>
        /// <typeparam name="TNotification">Type of notification
        /// </typeparam>
        /// <param name="notification">
        /// The notification.
        /// </param>
        protected void SetExpiration<TNotification>(TNotification notification)
        {
            if (this.Expiration != null)
            {
                this.ExpirationSetter(notification, this.Expiration.Value);
            }
        }
    }
}