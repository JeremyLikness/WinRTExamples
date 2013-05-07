// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupedItemsPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A page that displays a grouped collection of items.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer
{
    using System;
    using System.Collections.Generic;

    using TileExplorer.DataModel;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedItemsPage"/> class.
        /// </summary>
        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        public void HeaderClick(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var frameworkElement = sender as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            var group = frameworkElement.DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((TileGroup)@group).Name);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        public void ItemViewItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((TileItem)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
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
            var dataGroups = App.CurrentDataSource.GetGroups();
            this.DefaultViewModel["Groups"] = dataGroups;
        }
    }
}
