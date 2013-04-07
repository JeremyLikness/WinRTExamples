// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseItem.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RestServiceExample.DataModel
{
    using System;

    using RestServiceExample.Common;

    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// The base item.
    /// </summary>
    public abstract class BaseItem : BindableBase
    {
        /// <summary>
        /// The images.
        /// </summary>
        private static readonly Uri[] Images = new[]
                                                   {
                                                       new Uri("ms-appx:///Assets/DarkGray.png", UriKind.Absolute),
                                                       new Uri("ms-appx:///Assets/LightGray.png", UriKind.Absolute),
                                                       new Uri("ms-appx:///Assets/MediumGray.png", UriKind.Absolute)
                                                   };

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(Images[this.Id % Images.Length]);
            }
        }


        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// The equals method.
        /// </summary>
        /// <param name="obj">
        /// The object to compare to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> equality - must be same type and identifier.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType() == this.GetType() && 
                ((BaseItem)obj).Id == this.Id;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
