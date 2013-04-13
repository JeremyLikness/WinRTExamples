// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleDataCommon.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Base class for <see cref="SampleDataItem" /> and <see cref="SampleDataGroup" /> that
//   defines properties common to both.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SyndicationExample.DataModel
{
    using System;

    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Base class for <see cref="DataItem"/> and <see cref="DataFeed"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DataCommon : Common.BindableBase
    {
        /// <summary>
        /// The _base uri.
        /// </summary>
        private static readonly Uri BaseUri = new Uri("ms-appx:///");

        /// <summary>
        /// The _unique id.
        /// </summary>
        private string uniqueId = string.Empty;

        /// <summary>
        /// The title.
        /// </summary>
        private string title = string.Empty;

        /// <summary>
        /// The subtitle.
        /// </summary>
        private string subtitle = string.Empty;

        /// <summary>
        /// The description.
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// The image.
        /// </summary>
        private ImageSource image;

        /// <summary>
        /// The _image path.
        /// </summary>
        private string imagePath;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommon"/> class.
        /// </summary>
        /// <param name="uniqueId">
        /// The unique id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="imagePath">
        /// The image path.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        protected DataCommon(string uniqueId, string title, string subtitle, string imagePath, string description)
        {
            this.uniqueId = uniqueId;
            this.title = title;
            this.subtitle = subtitle;
            this.description = description;
            this.imagePath = imagePath;
        }
        
        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        public string UniqueId
        {
            get { return this.uniqueId; }
            set { this.SetProperty(ref this.uniqueId, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        public string Subtitle
        {
            get { return this.subtitle; }
            set { this.SetProperty(ref this.subtitle, value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public ImageSource Image
        {
            get
            {
                if (this.image == null && this.imagePath != null)
                {
                    this.image = new BitmapImage(new Uri(BaseUri, this.imagePath));
                }
                return this.image;
            }

            set
            {
                this.imagePath = null;
                this.SetProperty(ref this.image, value);
            }
        }

        /// <summary>
        /// The set image.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public void SetImage(String path)
        {
            this.image = null;
            this.imagePath = path;
            // ReSharper disable ExplicitCallerInfoArgument
            this.OnPropertyChanged("Image");
            // ReSharper restore ExplicitCallerInfoArgument
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Title;
        }
    }
}