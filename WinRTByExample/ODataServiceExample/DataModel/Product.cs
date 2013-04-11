// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Product.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The product.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODataServiceExample.DataModel
{
    /// <summary>
    /// The product.
    /// </summary>
    public class Product : BaseItem 
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public double Price { get; set; }
    }
}