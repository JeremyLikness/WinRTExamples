namespace ODataServiceExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ODataServiceExample.Common;
    using ODataServiceExample.DataModel;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage
    {
        public ItemDetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var itemId = (int)navigationParameter;
            var item =
                (from c in ((App)Application.Current).DataSource.Categories
                 from p in c.Products
                 where p.Id == itemId
                 select new { p, c }).FirstOrDefault();
            this.DefaultViewModel["Category"] = item.c;
            this.DefaultViewModel["Products"] = item.c.Products;
            this.flipView.SelectedItem = item.p;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedProduct = (Product)this.flipView.SelectedItem;
            if (selectedProduct != null)
            {
                pageState["SelectedItem"] = selectedProduct.Id;
            }
        }
    }
}
