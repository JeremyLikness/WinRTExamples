namespace TileExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TileExplorer.Common;
    using TileExplorer.DataModel;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage
    {
        private readonly string[] text = new[]
                                             {
                                                 "Tile Explorer", "by Jeremy Likness",
                                                 "Part of the WinRT by Examples book", "Automatically generates tiles.",
                                                 "Provides helper classes for tiles.", "Can update it's own tile.",
                                                 "Written in C#", "Standalone Windows Store app example.",
                                                 "Uses the Windows Runtime", "Full source code is available."
                                             };

        private readonly string[] images = new[]
                                               {
                                                   "ms-appx:///Assets/DarkGray.png", "ms-appx:///Assets/DarkGray.png",
                                                   "ms-appx:///Assets/DarkGray.png", "ms-appx:///Assets/DarkGray.png",
                                                   "ms-appx:///Assets/DarkGray.png", "ms-appx:///Assets/DarkGray.png"
                                               };


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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
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
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem != null)
            {
                pageState["SelectedItem"] = selectedItem.Id;
            }
        }

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

        private async void CopyCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.CopyOnClick();
        }

        private async void PinCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.SetOnClick();
        }
    }
}
