// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ODataServiceExample.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The data source.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// The service base.
        /// </summary>
        private static readonly Uri ServiceBase = new Uri("http://services.odata.org/OData/OData.svc", UriKind.Absolute);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class. 
        /// </summary>
        public DataSource()
        {
            this.Categories = new ObservableCollection<Category>();
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.InitializeWithSampleData();
            }
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public ObservableCollection<Category> Categories { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Initialize()
        {
            await this.InitializeWithServiceData();
        }

        /// <summary>
        /// The initialize with sample data.
        /// </summary>
        private void InitializeWithSampleData()
        {
            var product1 = new Product
                               {
                                   Description = "Delicious non-dairy milk.",
                                   Id = 1,
                                   Location = ServiceBase,
                                   Price = 1.99,
                                   Rating = 4,
                                   Title = "Almond Milk"
                               };

            var product2 = new Product 
                               {
                                   Description = "Tasty fresh-sequeezed orange juice.",
                                   Id = 2,
                                   Location = ServiceBase,
                                   Price = 2.99,
                                   Rating = 5,
                                   Title = "Orange Juice"
                               };

            var product3 = new Product
                               {
                                   Description = "Awesome Bluetooth headphones.",
                                   Id = 3,
                                   Location = ServiceBase,
                                   Price = 199.99,
                                   Rating = 3,
                                   Title = "Bluetooth Headphones"
                               };

            var category1 = new Category { Id = 1, Location = ServiceBase, Name = "Groceries" };
            category1.Products.Add(product1);
            category1.Products.Add(product2);
            this.Categories.Add(category1);
            
            var category2 = new Category { Id = 2, Location = ServiceBase, Name = "Electronics" };
            category2.Products.Add(product3);
            this.Categories.Add(category2);
        }

        /// <summary>
        /// The initialize with service data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task InitializeWithServiceData()
        {
            var client = new ODataService.DemoService(ServiceBase);

            var categoryQuery = client.Categories.AddQueryOption("$expand", "Products");

            var categories = await Task<IEnumerable<ODataService.Category>>
                .Factory.FromAsync(
                    categoryQuery.BeginExecute(result => { }, client),
                    categoryQuery.EndExecute);
           
            foreach (var item in categories)
            {
                var category = new Category { Id = item.ID, Name = item.Name, Location = ServiceBase };

                foreach (
                    var product in
                        item.Products.Select(
                            item1 =>
                            new Product
                                {
                                    Description = item1.Description,
                                    Id = item1.ID,
                                    Price = (double)item1.Price,
                                    Rating = item1.Rating,
                                    Title = item1.Name,
                                    Location = ServiceBase
                                }))
                {
                    category.Products.Add(product);
                }

                this.Categories.Add(category);
            }
        }
    }
}