// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkrapedPage.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The scraped page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System;
    using System.Collections.ObjectModel;

    using Skrape.Common;

    /// <summary>
    /// The scraped page.
    /// </summary>
    public class SkrapedPage : BasePropertyChange 
    {        
        /// <summary>
        /// The id.
        /// </summary>
        private int id;

        /// <summary>
        /// The url.
        /// </summary>
        private Uri url;

        /// <summary>
        /// The thumbnail path.
        /// </summary>
        private Uri thumbnailPath;

        /// <summary>
        /// The list of images
        /// </summary>
        private ObservableCollection<Uri> images;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;
        
        /// <summary>
        /// The HTML.
        /// </summary>
        private string html;

        /// <summary>
        /// The text.
        /// </summary>
        private string text;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkrapedPage"/> class.
        /// </summary>
        public SkrapedPage()
        {
            this.images = new ObservableCollection<Uri>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is loaded
        /// </summary>
        public bool Loaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page has been deleted.
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.url;
            }

            set
            {
                this.url = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        public Uri ThumbnailPath
        {
            get
            {
                return this.thumbnailPath;
            }

            set
            {
                this.thumbnailPath = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        public ObservableCollection<Uri> Images
        {
            get
            {
                return this.images;
            }

            set
            {
                this.images = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }        

        /// <summary>
        /// Gets or sets the html.
        /// </summary>
        public string Html
        {
            get
            {
                return this.html;
            }

            set
            {
                this.html = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                this.OnPropertyChanged();
            }
        }
    }
}
