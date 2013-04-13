// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemDetailPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A page that displays details for a single item within a group while allowing gestures to
//   flip through other items belonging to the same group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SyndicationExample
{
    using System;
    using System.Collections.Generic;

    using SyndicationExample.DataModel;

    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDetailPage"/> class.
        /// </summary>
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

            var item = DataSource.GetItem((string)navigationParameter);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
            var selectedItem = (DataItem)this.flipView.SelectedItem;
            if (selectedItem != null)
            {
                pageState["SelectedItem"] = selectedItem.UniqueId;
            }
        }
    }
}
