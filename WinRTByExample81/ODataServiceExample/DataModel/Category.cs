// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Category.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The category.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODataServiceExample.DataModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The category.
    /// </summary>
    public class Category : BaseItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        public Category()
        {
            Products = new ObservableCollection<Product>();
// ReSharper disable ExplicitCallerInfoArgument
            Products.CollectionChanged += (o, e) => OnPropertyChanged("TopProducts");
// ReSharper restore ExplicitCallerInfoArgument
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the list of products for this category
        /// </summary>
        public ObservableCollection<Product> Products { get; private set; }

        /// <summary>
        /// Gets the top products.
        /// </summary>
        public IEnumerable<Product> TopProducts
        {
            get
            {
                return Products.OrderBy(p => p.Title).Take(12);
            }
        }
    }
}