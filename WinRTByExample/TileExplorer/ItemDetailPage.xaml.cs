// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemDetailPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   A page that displays details for a single tile while allowing the user to scroll through other tiles
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TileExplorer.Common;
    using TileExplorer.DataModel;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.Foundation;
    using Windows.UI.Popups;
    using Windows.UI.StartScreen;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage
    {
        /// <summary>
        /// The text.
        /// </summary>
        private readonly string[] text = new[]
                                             {
                                                 "Tile Explorer", "by Jeremy Likness",
                                                 "WinRT by Examples", "Automatically generates tiles",
                                                 "Helper classes for tiles.", "Updates own tile",
                                                 "Written in C#", "Standalone Windows Store app",
                                                 "Uses Windows Runtime", "Full source code"
                                             };

        /// <summary>
        /// The images.
        /// </summary>
        private readonly string[] images = new[]
                                               {
                                                   "ms-appx:///Assets/slbookcover.png", "ms-appx:///Assets/buildingwind8cover.jpg",
                                                   "ms-appx:///Assets/avatar.png", "ms-appx:///Assets/paris.jpg",
                                                   "http://gallery.jeremylikness.com/main.php?g2_view=core.DownloadItem&g2_itemId=273&g2_serialNumber=1", 
                                                   "http://lh5.ggpht.com/--mPuxdvKqf8/USFpzDUXXiI/AAAAAAAAA5s/DCz4EuXvIn8/s1600-h/keyboard3.jpg"
                                               };

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

            var item = App.CurrentDataSource.GetTile((string)navigationParameter);
            var itemGroup = App.CurrentDataSource.GetGroupForItem(item.Id);
            this.DefaultViewModel["Group"] = itemGroup;
            this.DefaultViewModel["Items"] = itemGroup.Items;            
            this.flipView.SelectedItem = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem != null)
            {
                pageState["SelectedItem"] = selectedItem.Id;
            }
        }

        /// <summary>
        /// The copy on click.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task CopyOnClick()
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            var dataPackage = new DataPackage();
            dataPackage.SetText(selectedItem.Tile.ToString());

            var content = "The XML for the tile has been copied to the clipboard.";
            var title = "Success";

            try
            {
                // Set the DataPackage to clipboard. 
                Clipboard.SetContent(dataPackage);                
            }
            catch (Exception ex)
            {
                // Copying data to Clipboard can potentially fail - for example, if another application is holding Clipboard open 
                content = ex.Message;
                title = "Copy to Clipboard Failed";
            }

            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The set on click.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task SetOnClick()
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            var content = "The tile was successfully set.";
            var title = "Success";

            try
            {
                var baseTile = selectedItem.Tile;

                for (var textLines = 0; textLines < baseTile.TextLines; textLines++)
                {
                    baseTile.AddText(this.text[textLines]);
                }

                for (var imageIndex = 0; imageIndex < baseTile.Images; imageIndex++)
                {
                    baseTile.AddImage(this.images[imageIndex], "Sample Image");
                }

                baseTile.Set();
            }
            catch (Exception ex)
            {
                title = "Error";
                content = ex.Message;
            }

            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The set on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task PinOnClick(object sender)
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            var content = "The tile was successfully pinned.";
            var title = "Success";

            try
            {
                var logo = new Uri("ms-appx:///Assets/Logo.png");
                var smallLogo = new Uri("ms-appx:///Assets/SmallLogo.png");
                var wideLogo = new Uri("ms-appx:///Assets/WideLogo.png");
                
                var tile = new SecondaryTile(
                    selectedItem.Id, 
                    selectedItem.Id, 
                    selectedItem.Description, 
                    string.Format("Id={0}", selectedItem.Id), 
                    TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo, 
                    logo)
                               {
                                   ForegroundText = ForegroundText.Light, 
                                   SmallLogo = smallLogo, 
                                   WideLogo = wideLogo
                               };

                var appBarButton = sender as FrameworkElement;
                if (appBarButton != null)
                {
                    var transformation = appBarButton.TransformToVisual(null);
                    var point = transformation.TransformPoint(new Point());
                    var rect = new Rect(point, new Size(appBarButton.ActualWidth, appBarButton.ActualHeight));
                    await tile.RequestCreateForSelectionAsync(rect, Placement.Above);
                }
                else
                {
                    throw new Exception("Could not aquire source element for positioning.");
                }
            }
            catch (Exception ex)
            {
                title = "Error";
                content = ex.Message;
            }

            var dialog = new MessageDialog(content, title);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The home command_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HomeCommand_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GroupedItemsPage), "AllGroups");
        }

        /// <summary>
        /// The copy command_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void CopyCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.CopyOnClick();
        }

        /// <summary>
        /// The pin command_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void SetCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.SetOnClick();
        }

        /// <summary>
        /// The pin command_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void PinCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.PinOnClick(sender);
        }
    }
}
