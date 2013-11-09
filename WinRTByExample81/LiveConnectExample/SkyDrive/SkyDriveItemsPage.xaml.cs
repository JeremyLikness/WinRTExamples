using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
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
        #region Fields

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly IDialogService _dialogService;

        private readonly LiveConnectWrapper _liveConnectWrapper;

        private String _skyDriveItemId;
        private readonly ObservableCollection<dynamic> _skydriveItems = new ObservableCollection<dynamic>();

        private readonly RelayCommand _refreshCommand;
        private readonly RelayCommand _openCreateFolderCommand;
        private readonly RelayCommand _createFolderCommand;
        private readonly RelayCommand _uploadCommand;

        // Commands for one or more current items
        private readonly RelayCommand _deleteItemCommand;
        private readonly RelayCommand _openRenameCommand;
        private readonly RelayCommand _renameItemCommand;
        private readonly RelayCommand _downloadCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyDriveItemsPage"/> class.
        /// </summary>
        public SkyDriveItemsPage()
        {
            InitializeComponent();

            _dialogService = new DialogService(Dispatcher);
            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;

            _refreshCommand = new RelayCommand(Refresh);
            _openCreateFolderCommand = new RelayCommand(OpenCreateFolder);
            _createFolderCommand = new RelayCommand(CreateFolder, CanCreateFolder);
            _uploadCommand = new RelayCommand(UploadFile);

            // Commands for one or more current items
            _deleteItemCommand = new RelayCommand(DeleteItem, CanDeleteItem);
            _openRenameCommand = new RelayCommand(OpenRenameItem, CanOpenRenameItem);
            _renameItemCommand = new RelayCommand(RenameItem, CanRenameItem);
            _downloadCommand = new RelayCommand(DownloadItem, CanDownloadItem);

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        } 

        #endregion

        #region State Support

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

        #endregion

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
            DefaultViewModel["SelectedItem"] = null;

            DefaultViewModel["RefreshCommand"] = _refreshCommand;
            DefaultViewModel["OpenCreateFolderCommand"] = _openCreateFolderCommand;
            DefaultViewModel["CreateFolderCommand"] = _createFolderCommand;
            DefaultViewModel["UploadCommand"] = _uploadCommand;
            
            // Commands for one or more current items
            DefaultViewModel["DeleteItemCommand"] = _deleteItemCommand;
            DefaultViewModel["OpenRenameCommand"] = _openRenameCommand;
            DefaultViewModel["RenameItemCommand"] = _renameItemCommand;
            DefaultViewModel["DownloadCommand"] = _downloadCommand;

            DefaultViewModel.MapChanged += (sender, @event) =>
                                           {
                                               if (@event.Key == "FolderName") _createFolderCommand.RaiseCanExecuteChanged();
                                               if (@event.Key == "ItemNewName") _renameItemCommand.RaiseCanExecuteChanged();
                                           };

            _liveConnectWrapper.SessionChanged += OnLiveConnectWrapperSessionChanged;
            await UpdateContent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);

            _liveConnectWrapper.SessionChanged -= OnLiveConnectWrapperSessionChanged;
        }

        #endregion

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

        #region Load Content

        private async void OnLiveConnectWrapperSessionChanged(Object sender, EventArgs eventArgs)
        {
            await UpdateContent();
        }

        private async void Refresh()
        {
            await UpdateContent();
        }

        private async Task UpdateContent()
        {
            var isConnected = _liveConnectWrapper.IsSessionAvailable;
            DefaultViewModel["IsConnected"] = isConnected;

            if (isConnected)
            {
                var skyDriveItem = await _liveConnectWrapper.GetSkyDriveItemAsync(_skyDriveItemId);
                DefaultViewModel["SkyDriveItem"] = skyDriveItem;
                DefaultViewModel["SelectedItem"] = null;
                DefaultViewModel["IsFolderOrAlbum"] = skyDriveItem.type.ToString().Equals("folder") || skyDriveItem.type.ToString().Equals("album");

                var skyDriveItemValues = new Dictionary<String, Object>(skyDriveItem as IDictionary<String, Object>);
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
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }

            if (skyDriveItem.type.ToString().Equals("album"))
            {
                var albumImageUrl = await _liveConnectWrapper.GetAlbumPictureUrlAsync(_skyDriveItemId);
                DefaultViewModel["ProfileImageSource"] = albumImageUrl;
            }
        }

        private async void LoadPhotoContent()
        {
            try
            {
                var profileImageUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Small);
                DefaultViewModel["ProfileImageSource"] = profileImageUrl;

                var itemPictureUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Full);
                DefaultViewModel["SkyDrivePhotoUrl"] = itemPictureUrl;
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private async void LoadMediaContent(dynamic skyDriveItem)
        {
            try
            {
                var itemPictureUrl = await _liveConnectWrapper.GetSkydriveItemPictureAsync(_skyDriveItemId, LiveConnectWrapper.PictureSize.Small);
                DefaultViewModel["ProfileImageSource"] = itemPictureUrl;
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }

            var mediaUri = new Uri(skyDriveItem.source);
            DefaultViewModel["SkyDriveMediaUrl"] = mediaUri;
        }

        private async void LoadFileContent(dynamic skyDriveItem)
        {
            // Only load content that is "embeddable"
            if (!skyDriveItem.is_embeddable) return;

            try
            {
                var fileUrl = await _liveConnectWrapper.GetSkydriveItemLinkUrlAsync(_skyDriveItemId);
                DefaultViewModel["SkyDriveEmbeddableItemUrl"] = fileUrl;

            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        #endregion

        #region Command Support

        private void OpenCreateFolder()
        {
            DefaultViewModel["FolderName"] = LiveConnectWrapper.DefaultNewFolderName;
        }

        private async void CreateFolder()
        {
            try
            {
                var folderName = (DefaultViewModel["FolderName"] ?? String.Empty).ToString();
                var newFolder = await _liveConnectWrapper.CreateSkyDriveFolderAsync(_skyDriveItemId, folderName);
                _skydriveItems.Add(newFolder);
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private Boolean CanCreateFolder()
        {
            return !String.IsNullOrWhiteSpace(DefaultViewModel["FolderName"] as String);
        }

        private async void UploadFile()
        {
            var picker = new FileOpenPicker
                         {
                             ViewMode = PickerViewMode.Thumbnail,
                             SuggestedStartLocation = PickerLocationId.PicturesLibrary
                         };
            picker.FileTypeFilter.Add("*");
            
            var fileToUpload = await picker.PickSingleFileAsync();

            var currentQuota = await _liveConnectWrapper.GetUserSkyDriveQuotaAsync();
            var fileInfo = await fileToUpload.GetBasicPropertiesAsync();
            if (currentQuota.Available < 0 || (UInt64) currentQuota.Available < fileInfo.Size)
            {
                _dialogService.ShowError("There is not enough space available in the SkyDrive account to store the file.");
                return;
            }

            dynamic currentItem = DefaultViewModel["SkyDriveItem"];
            try
            {
                var cancellationToken = new CancellationTokenSource();
                var progressHandler = new Progress<LiveOperationProgress>(progress => 
                                            {
                                                if (progress.ProgressPercentage >= 100)
                                                {
                                                    //_dialogService.ShowMessageBox("Upload completed.", "Upload");
                                                }
                                            });
                dynamic uploadResult = await _liveConnectWrapper.StartBackgroundUploadAsync(currentItem.id, fileToUpload.Name, fileToUpload, cancellationToken.Token, progressHandler);
                var uploadedItemId = uploadResult.id;
                AddUploadedItemToList(uploadedItemId);
                _dialogService.ShowMessageBox("Upload completed.", "Upload");
            }
            catch (TaskCanceledException)
            {
                _dialogService.ShowMessageBox("Upload cancelled.", "Upload");
            }
            catch (LiveConnectException exception)
            {
                _dialogService.ShowError("Error uploading file: " + exception.Message);
            }
        }

        private async void AddUploadedItemToList(String uploadedItemId)
        {
            try
            {
                dynamic addedItem = await _liveConnectWrapper.GetSkyDriveItemAsync(uploadedItemId);
                _skydriveItems.Add(addedItem);

            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private void DeleteItem()
        {
            dynamic selectedItem = DefaultViewModel["SelectedItem"];
            var itemName = selectedItem.name;

            var removeCommand = new UICommand("Remove", command => RemoveItem(selectedItem));
            var cancelCommand = new UICommand("Cancel");
            var commands = new[] { removeCommand, cancelCommand };
            _dialogService.ShowMessageBoxAsync("Are you sure you want to remove item " + itemName, "Remove Item?", commands, 1);
        }

        private async void RemoveItem(dynamic selectedItem)
        {
            try
            {
                await _liveConnectWrapper.DeleteSkyDriveItemAsync(selectedItem.id);
                _skydriveItems.Remove(selectedItem);
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private Boolean CanDeleteItem()
        {
            return DefaultViewModel["SelectedItem"] != null;
        }

        private void OpenRenameItem()
        {
            dynamic skyDriveItem = DefaultViewModel["SelectedItem"];
            DefaultViewModel["ItemNewName"] = skyDriveItem.name.ToString();
        }

        private Boolean CanOpenRenameItem()
        {
            return DefaultViewModel["SelectedItem"] != null;
        }

        private async void RenameItem()
        {
            try
            {
                var itemNewName = (DefaultViewModel["ItemNewName"] ?? String.Empty).ToString();
                dynamic selectedItem = DefaultViewModel["SelectedItem"];
                var itemId = selectedItem.id;
                var updatedItem = await _liveConnectWrapper.RenameSkyDriveItemAsync(itemId, itemNewName);
                var index = _skydriveItems.IndexOf(selectedItem);
                _skydriveItems[index] = updatedItem;
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }

        private Boolean CanRenameItem()
        {
            dynamic selectedItem = DefaultViewModel["SelectedItem"];
            var itemOriginalName = selectedItem.name;
            var itemNewName = DefaultViewModel["ItemNewName"] as String;
            return (!String.IsNullOrWhiteSpace(itemNewName))
                   && !(itemOriginalName.Equals(itemNewName));
        }

        private async void DownloadItem()
        {
            dynamic downloadItem = DefaultViewModel["SelectedItem"];
            if (downloadItem == null)
            {
                dynamic currentItem = DefaultViewModel["SkyDriveItem"];
                if (currentItem.type != "folder" && currentItem.type != "album")
                {
                    downloadItem = currentItem;
                }
            }
            if (downloadItem == null) return;

            var picker = new FileSavePicker
                         {
                             SuggestedFileName = downloadItem.name,
                             SuggestedStartLocation = PickerLocationId.Downloads
                         };
            var fileExtension = Path.GetExtension(downloadItem.name);
            picker.FileTypeChoices.Add("File", new List<String> {fileExtension});
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    var cancellationToken = new CancellationTokenSource();
                    var progressHandler = new Progress<LiveOperationProgress>(progress =>
                                                {
                                                    DefaultViewModel["DownloadProgress"] = progress;
                                                    if (progress.ProgressPercentage >= 100)
                                                    {
                                                        _dialogService.ShowMessageBox("Download completed.", "Download");
                                                    }
                                                });
                    await _liveConnectWrapper.StartBackgroundDownloadAsync(downloadItem.id, file, cancellationToken.Token, progressHandler);
                }
                catch (TaskCanceledException)
                {
                    _dialogService.ShowMessageBox("Download cancelled.", "Download");
                }
                catch (LiveConnectException exception)
                {
                    _dialogService.ShowError("Error downloading file: " + exception.Message);
                }
            }
        }

        private Boolean CanDownloadItem()
        {
            dynamic downloadItem = DefaultViewModel["SelectedItem"];
            if (downloadItem == null)
            {
                dynamic currentItem = DefaultViewModel["SkyDriveItem"];
                if (currentItem.type != "folder" && currentItem.type != "album")
                {
                    downloadItem = currentItem;
                }
            }
            return downloadItem != null;
        } 
        #endregion

        private void HandleSkyDriveItemClicked(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var skydriveItem = ((dynamic) e.ClickedItem);
            String skydriveItemId = skydriveItem.id;
            Frame.Navigate(typeof(SkyDriveItemsPage), skydriveItemId);
        }

        private void HandleSkyDriveItemSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault();
            DefaultViewModel["SelectedItem"] = selectedItem;
            
            _downloadCommand.RaiseCanExecuteChanged();
            _openRenameCommand.RaiseCanExecuteChanged();
            _deleteItemCommand.RaiseCanExecuteChanged();
        }
    }
}
