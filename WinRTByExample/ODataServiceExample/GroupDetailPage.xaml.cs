﻿namespace ODataServiceExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ODataServiceExample.DataModel;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDetailPage"/> class.
        /// </summary>
        public GroupDetailPage()
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
            var categoryId = (int)navigationParameter;
            var category = ((App)Application.Current).DataSource.Categories.FirstOrDefault(c => c.Id == categoryId);
            this.DefaultViewModel["Category"] = category;
            this.DefaultViewModel["Products"] = category.Products;
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        private void ItemViewItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Product)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }
    }
}
