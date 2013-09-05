// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkrapeGroup.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A group of scraped items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    using System.Collections.ObjectModel;

    using Common;

    /// <summary>
    /// A group of scraped items.
    /// </summary>
    public class SkrapeGroup : BasePropertyChange 
    {       
        /// <summary>
        /// The id.
        /// </summary>
        private int id;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkrapeGroup"/> class.
        /// </summary>
        public SkrapeGroup()
        {
            Pages = new ObservableCollection<SkrapedPage>();
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
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the pages.
        /// </summary>
        public ObservableCollection<SkrapedPage> Pages { get; set; }        
    }
}
