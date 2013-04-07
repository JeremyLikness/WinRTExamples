// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The data source.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RestServiceExample.DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Windows.Data.Json;

    /// <summary>
    /// The data source.
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// The categories uri.
        /// </summary>
        private const string CategoriesUri = "OData.svc/Categories";

        /// <summary>
        /// The service base.
        /// </summary>
        private static readonly Uri ServiceBase = new Uri("http://services.odata.org/OData/", UriKind.Absolute);

        /// <summary>
        /// The JSON media type.
        /// </summary>
        private static readonly MediaTypeWithQualityHeaderValue Json = new MediaTypeWithQualityHeaderValue("application/json");

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
        /// The load products for category.
        /// </summary>
        /// <param name="productsUri">
        /// The products uri.
        /// </param>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task LoadProductsForCategory(Uri productsUri, Category category)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(Json);

            var jsonResponse = await client.GetStringAsync(productsUri);

            var json = JsonObject.Parse(jsonResponse);
            var productsList = json["d"].GetArray();
            foreach (var entry in productsList)
            {
                var productJson = entry.GetObject();
                var metadata = productJson["__metadata"].GetObject();
                var productUri = metadata["uri"].GetString();

                var product = new Product
                                  {
                                      Id = (int)productJson["ID"].GetNumber(),
                                      Title = productJson["Name"].GetString(),
                                      Description = productJson["Description"].GetString(),
                                      Price = double.Parse(productJson["Price"].GetString()),
                                      Rating = (int)productJson["Rating"].GetNumber(),
                                      Location = new Uri(productUri, UriKind.Absolute)
                                  };

                category.Products.Add(product);
            }
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
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(Json);

            var uri = new Uri(ServiceBase, CategoriesUri);

            var jsonResponse = await client.GetStringAsync(uri);

            var json = JsonObject.Parse(jsonResponse);
            var d = json["d"].GetArray();

            if (d == null)
            {
                return;
            }

            foreach (var entry in d)
            {
                var categoryJson = entry.GetObject();
                var metadata = categoryJson["__metadata"].GetObject();
                var categoryUri = metadata["uri"].GetString();
                var category = new Category
                {
                    Id = (int)categoryJson["ID"].GetNumber(),
                    Name = categoryJson["Name"].GetString(),
                    Location = new Uri(categoryUri, UriKind.Absolute)
                };
                this.Categories.Add(category);
                var productsUri = categoryJson["Products"]
                    .GetObject()["__deferred"]
                    .GetObject()["uri"].GetString();
                await LoadProductsForCategory(new Uri(productsUri, UriKind.Absolute), category);
            }
        }
    }
}