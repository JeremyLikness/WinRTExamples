// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleDataGroup.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Generic group data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SyndicationExample.DataModel
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class DataFeed : DataCommon
    {
        /// <summary>
        /// The items.
        /// </summary>
        private readonly ObservableCollection<DataItem> items = new ObservableCollection<DataItem>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFeed"/> class.
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
        public DataFeed(string uniqueId, string title, string subtitle, string imagePath, string description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {            
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public ObservableCollection<DataItem> Items
        {
            get { return this.items; }
        }        
    }
}