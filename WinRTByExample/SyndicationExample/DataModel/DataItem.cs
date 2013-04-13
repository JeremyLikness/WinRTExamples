// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleDataItem.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Generic item data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SyndicationExample.DataModel
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class DataItem : DataCommon
    {
        /// <summary>
        /// The content.
        /// </summary>
        private string content = string.Empty;

        /// <summary>
        /// The group.
        /// </summary>
        private DataFeed @group;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItem"/> class.
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
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="group">
        /// The group.
        /// </param>
        public DataItem(string uniqueId, string title, string subtitle, string imagePath, string description, string content, DataFeed group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this.content = content;
            this.@group = group;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content
        {
            get { return this.content; }
            set { this.SetProperty(ref this.content, value); }
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public DataFeed Group
        {
            get { return this.@group; }
            set { this.SetProperty(ref this.@group, value); }
        }
    }
}