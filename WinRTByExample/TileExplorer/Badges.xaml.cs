// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Badges.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A basic page that allows updating the badge
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TileExplorer.Common;
    using TileExplorer.DataModel;

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using WinRTByExample.NotificationHelper.Badges;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class Badges
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Badges"/> class.
        /// </summary>
        public Badges()
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
            this.DefaultViewModel["Items"] = App.CurrentDataSource.Badges;
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
        /// The set badge_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void SetBadge_OnClick(object sender, RoutedEventArgs e)
        {
            var numeric = GlyphOrNumeric.IsOn;
            var title = "Success";
            string message;

            try
            {
                if (numeric)
                {
                    var badgeValue = (int)NumericSlider.Value;
                    badgeValue.GetBadge().Set();
                    message = "Numeric badge was set.";
                }
                else
                {
                    var item = this.Glyphs.SelectedItem as BadgeItem ?? App.CurrentDataSource.Badges.First();
                    item.Badge.Set();
                    message = "Glyph badge was set.";
                }
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
            this.Frame.GoBack();
        }

        /// <summary>
        /// The clear badge_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void ClearBadge_OnClick(object sender, RoutedEventArgs e)
        {
            BadgeHelper.ClearBadge().Set();
            var dialog = new MessageDialog("The badge was cleared.");
            await dialog.ShowAsync();
            this.Frame.GoBack();
        }
    }
}
