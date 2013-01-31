﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="List.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A basic page that provides characteristics common to most applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StateManagementExample
{
    using System;
    using System.Collections.Generic;

    using StateManagementExample.Common;

    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class List
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// </summary>
        public List()
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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
        }

        /// <summary>
        /// The selector_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ItemsList.SelectedItem as ListViewItem;
            if (item == null)
            {
                return;
            }

            var id = item.Content as string;
            if (!string.IsNullOrEmpty(id))
            {
                this.Frame.Navigate(typeof(Item), id);
            }
        }
    }
}
