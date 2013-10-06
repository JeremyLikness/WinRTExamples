using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using WinRTByExample.NotificationHelper.Tiles;

namespace TileExplorer
{
    using Common;
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using DataModel;

    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class ItemDetailPage
    {
        /// <summary>
        /// The text.
        /// </summary>
        private readonly string[] text =
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
        private readonly string[] images =
        {
            "ms-appx:///Assets/slbookcover.png", "ms-appx:///Assets/buildingwind8cover.jpg",
            "ms-appx:///Assets/avatar.png", "ms-appx:///Assets/paris.jpg",
            "http://gallery.jeremylikness.com/main.php?g2_view=core.DownloadItem&g2_itemId=273&g2_serialNumber=1", 
            "http://lh5.ggpht.com/--mPuxdvKqf8/USFpzDUXXiI/AAAAAAAAA5s/DCz4EuXvIn8/s1600-h/keyboard3.jpg"
        };

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// Set the notification button icon
        /// </summary>
        /// <param name="sender">The button to set</param>
        private static void SetNotificationButtonIcon(object sender)
        {
            ((AppBarButton) sender).Icon = App.NotificationsOn
                ? new SymbolIcon(Symbol.Stop)
                : new SymbolIcon(Symbol.Play);
            ((AppBarButton) sender).Label = App.NotificationsOn
                ? "Stop"
                : "Start";
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
                var baseTile = new BaseTile(selectedItem.Tile.Type);

                for (var textLines = 0; textLines < baseTile.TextLines; textLines++)
                {
                    baseTile.AddText(this.text[textLines]);
                }

                for (var imageIndex = 0; imageIndex < baseTile.Images; imageIndex++)
                {
                    baseTile.AddImage(this.images[imageIndex], "Sample Image");
                }

                baseTile.WithNoBranding().Set();
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
                var logo150X150 = new Uri("ms-appx:///Assets/Logo.png");
                var logo30X30 = new Uri("ms-appx:///Assets/SmallLogo.png");
                var logo310X150 = new Uri("ms-appx:///Assets/WideLogo.png");

                var tile = new SecondaryTile(
                    selectedItem.Id,
                    selectedItem.Id,
                    string.Format("Id={0}", selectedItem.Id),
                    logo150X150,
                    TileSize.Square150x150);

                tile.VisualElements.ForegroundText = ForegroundText.Light;
                tile.VisualElements.Square30x30Logo = logo30X30;
                tile.VisualElements.Wide310x150Logo = logo310X150;
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                
                var appBarButton = sender as FrameworkElement;
                if (appBarButton != null)
                {
                    var transformation = appBarButton.TransformToVisual(null);
                    var point = transformation.TransformPoint(new Point());
                    var rect = new Rect(point, new Size(appBarButton.ActualWidth, appBarButton.ActualHeight));
                    var success = await tile.RequestCreateForSelectionAsync(rect, Placement.Above);
                    if (!success)
                    {
                        title = "Canceled";
                        content = "The pin request was canceled.";
                    }
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
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public ItemDetailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            var selectedItem = (TileItem)this.flipView.SelectedItem;
            if (selectedItem != null)
            {
                e.PageState["SelectedItem"] = selectedItem.Id;
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var navigationParameter = e.PageState != null && e.PageState.ContainsKey("SelectedItem")
                ? e.PageState["SelectedItem"]
                : e.NavigationParameter;
            var item = App.CurrentDataSource.GetTile((string) navigationParameter);
            var itemGroup = App.CurrentDataSource.GetGroupForItem(item.Id);
            this.DefaultViewModel["Group"] = itemGroup;
            this.DefaultViewModel["Items"] = itemGroup.Items;
            this.flipView.SelectedItem = item;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void HomeCommand_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (GroupedItemsPage), "AllGroups");
        }

        private void NotificationButton_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetNotificationButtonIcon(sender);
        }

        private async void NotificationCommand_OnClick(object sender, RoutedEventArgs e)
        {
            var current = App.NotificationsOn;

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(!current);
            App.NotificationsOn = !current;
            var message = current ? "Notifications have been turned off." : "Notifications have been turned on. Tiles should cycle.";
            SetNotificationButtonIcon(sender);

            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private async void CopyCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.CopyOnClick();
        }

        private async void SetCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.SetOnClick();
        }

        private async void PinCommand_OnClick(object sender, RoutedEventArgs e)
        {
            await this.PinOnClick(sender);
        }
    }
}