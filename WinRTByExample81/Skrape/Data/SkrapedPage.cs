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

    using Common;

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
            images = new ObservableCollection<Uri>();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                OnPropertyChanged();
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
                return url;
            }

            set
            {
                url = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        public Uri ThumbnailPath
        {
            get
            {
                return thumbnailPath;
            }

            set
            {
                thumbnailPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        public ObservableCollection<Uri> Images
        {
            get
            {
                return images;
            }

            set
            {
                images = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
                OnPropertyChanged();
            }
        }        

        /// <summary>
        /// Gets or sets the html.
        /// </summary>
        public string Html
        {
            get
            {
                return html;
            }

            set
            {
                html = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                OnPropertyChanged();
            }
        }
    }
}
