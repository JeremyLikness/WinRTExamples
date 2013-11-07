using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LiveConnectExample.Common;
using Microsoft.Live;

namespace LiveConnectExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SkyDriveItemsPage : Page
    {

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        private readonly LiveConnectWrapper _liveConnectWrapper;

        private String _skyDriveItemId;
        private readonly ObservableCollection<dynamic> _skydriveItems = new ObservableCollection<dynamic>();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        public SkyDriveItemsPage()
        {
            InitializeComponent();

            DefaultViewModel["RefreshCommand"] = new RelayCommand(Refresh);

            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            _skyDriveItemId = e.Parameter as String;
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;
            DefaultViewModel["ProfileImageSource"] = new Uri("ms-appx:///Assets/SkyDriveIconWhite.png");
            DefaultViewModel["SkyDriveItem"] = null;
            DefaultViewModel["SkyDriveItems"] = _skydriveItems;

            _liveConnectWrapper.SessionChanged += OnLiveConnectWrapperSessionChanged;
            await UpdateContent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);

            _liveConnectWrapper.SessionChanged -= OnLiveConnectWrapperSessionChanged;
        }

        #endregion


        private async void OnLiveConnectWrapperSessionChanged(Object sender, EventArgs eventArgs)
        {
            await UpdateContent();
        }

        private async Task UpdateContent()
        {
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;

            if (_liveConnectWrapper.IsSessionAvailable)
            {
                var skyDriveItem = await _liveConnectWrapper.GetSkyDriveItemAsync(_skyDriveItemId);
                DefaultViewModel["SkyDriveItem"] = skyDriveItem;

                var skyDriveItemValues =
                    new Dictionary<String, Object>(skyDriveItem as IDictionary<String, Object>);
                skyDriveItemValues.Remove("id");
                skyDriveItemValues.Remove("name");

                var profileItemsList = skyDriveItemValues.FlattenDynamicItems(String.Empty);
                DefaultViewModel["AdditionalDetails"] = profileItemsList.Select(x => new { x.Key, x.Value }).ToList();

                String itemType = skyDriveItem.type.ToString();
                switch (itemType)
                {
                    case "folder":
                    case "album":
                        LoadFolderOrAlbumContent(skyDriveItem);
                        break;
                    case "photo":
                        LoadPhotoContent();
                        break;
                    case "audio":
                    case "video":
                        LoadMediaContent(skyDriveItem);
                        break;
                    case "file":
                    case "notebook":
                        LoadFileContent(skyDriveItem);
                        break;
                }
            }
        }

        private async void LoadFolderOrAlbumContent(dynamic skyDriveItem)
        {
            try
            {
                var skyDriveItemContents =
                    await _liveConnectWrapper.GetSkydriveItemContentsAsync(_skyDriveItemId);
                var orderedSkyDriveContents =
                    new List<dynamic>(skyDriveItemContents).OrderBy(
                        x => ((String)x.type).GetSkyDriveItemTypeOrder()).ThenBy(x => x.name);
                _skydriveItems.Clear();
                foreach (var item in orderedSkyDriveContents)
                {
                    _skydriveItems.Add(item);
                }
            }
            catch (LiveConnectException)
            {
                // TODO - Display error information in the UI (likely a scopes issue)
            }

            if (skyDriveItem.type.ToString().Equals("album"))
            {
                var albumImageUrl = await _liveConnectWrapper.GetAlbumPictureUrlAsync(_skyDriveItemId);
                DefaultViewModel["ProfileImageSource"] = albumImageUrl;
            }
        }

        private async void LoadPhotoContent()
        {
            var profileImageUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Small);
            DefaultViewModel["ProfileImageSource"] = profileImageUrl;
            
            var itemPictureUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Full);
            DefaultViewModel["SkyDrivePhotoUrl"] = itemPictureUrl;
        }

        private async void LoadMediaContent(dynamic skyDriveItem)
        {
            var itemPictureUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Small);
            DefaultViewModel["ProfileImageSource"] = itemPictureUrl;

            var mediaUri = new Uri(skyDriveItem.source);
            DefaultViewModel["SkyDriveMediaUrl"] = mediaUri;
        }

        private async void LoadFileContent(dynamic skyDriveItem)
        {
            if (skyDriveItem.is_embeddable)
            {
                var fileUrl = await _liveConnectWrapper.GetSkydriveItemLinkUrlAsync(_skyDriveItemId);
                DefaultViewModel["SkyDriveEmbeddableItemUrl"] = fileUrl;
            }
        }

        private async void Refresh()
        {
            await UpdateContent();
        }

        private void HandleSkyDriveItemClicked(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var skydriveItem = ((dynamic) e.ClickedItem);
            String skydriveItemId = skydriveItem.id;
            Frame.Navigate(typeof(SkyDriveItemsPage), skydriveItemId);
        }
    }

    public class SkyDriveContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderOrAlbumTemplate { get; set; }

        public DataTemplate AudioVideoTemplate { get; set; }

        public DataTemplate PhotoTemplate { get; set; }

        public DataTemplate EmbeddableFileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(Object item, DependencyObject container)
        {
            var skyDriveItem = (dynamic) item;
            String itemType = item == null ? String.Empty : skyDriveItem.type.ToString();
            switch (itemType)
            {
                case "folder":
                case "album":
                    return FolderOrAlbumTemplate;
                case "photo":
                    return PhotoTemplate;
                case "video":
                case "audio":
                    return AudioVideoTemplate;
                case "file":
                    return EmbeddableFileTemplate;
                default:
                    return base.SelectTemplateCore(item, container);
            }
        }
    }
}
