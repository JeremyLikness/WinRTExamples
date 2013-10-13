using System;
using System.Collections.Generic;
using Windows.Media.PlayTo;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using PlayToExample.Common;

namespace PlayToExample
{
    /// <summary>
    /// The page to stream a collection of pictures to a selected PlayTo device.
    /// </summary>
    public sealed partial class PicturesPage : Page
    {

        private readonly NavigationHelper _navigationHelper;
        private Image _currentPlayToImage;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        public PicturesPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var playToManager = PlayToManager.GetForCurrentView();
            playToManager.SourceRequested += OnPlayToSourceRequested;
            //playToManager.SourceSelected += OnPlayToSourceSelected;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var playToManager = PlayToManager.GetForCurrentView();
            playToManager.SourceRequested -= OnPlayToSourceRequested;
            //playToManager.SourceSelected -= OnPlayToSourceSelected;
        }

        #endregion

        private void OnPlayToSourceRequested(PlayToManager sender, PlayToSourceRequestedEventArgs args)
        {
            //args.SourceRequest.DisplayErrorString("Test error string");
            var deferral = args.SourceRequest.GetDeferral();
            // This request will come in on a non-UI thread, so it will need to be marshalled over.  
            // Since doing that is an an async operation, a deferral will be required.
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    _currentPlayToImage = (Image)SelectedImageContainer.Content;
                    if (_currentPlayToImage == null)
                    {
                        args.SourceRequest.DisplayErrorString("There is no image selected to be streamed.");
                    }
                    else
                    {
                        args.SourceRequest.SetSource(_currentPlayToImage.PlayToSource);
                    }
                    deferral.Complete();
                });
        }

        //private void OnPlayToSourceSelected(PlayToManager sender, PlayToSourceSelectedEventArgs args)
        //{
        //    var playToTargetName = args.FriendlyName;
        //    var playToTargetDisplayIcon = args.Icon;
        //    var targetSupportsAudio = args.SupportsAudio;
        //    var targetSupportsVideo = args.SupportsVideo;
        //    var targetSupportsImage = args.SupportsImage;
        //}

        private async void HandleSelectPicturesClicked(Object sender, RoutedEventArgs e)
        {
            // Select the files from the "file system" (uses the picker, so they could be anywhere...)
            var filePicker = new FileOpenPicker
                             {
                                 SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                                 FileTypeFilter = {".png", ".jpg", ".bmp"},
                                 ViewMode = PickerViewMode.Thumbnail,
                             };
            var selectedFiles = await filePicker.PickMultipleFilesAsync();

            // From the selected files, get the corresponding BitmapImage objects.
            var selectedImages = new List<BitmapImage>();
            foreach (var storageFile in selectedFiles)
            {
                var image = new BitmapImage();
                var stream = await storageFile.OpenReadAsync();
                image.SetSource(stream);
                selectedImages.Add(image);
            }

            // Set the new list of items to be the source for the list shown in the UI
            ImagesListView.ItemsSource = selectedImages;
            ImagesListView.SelectedIndex = 0;
        }

        private void HandleImagesListViewSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            var selectedBitmapImage = (BitmapImage)ImagesListView.SelectedItem;
            var newImageElement = new Image {Source = selectedBitmapImage};
            
            // If in the middle of a PlayTo, set the new image as the next to play and instruct it to be played.
            if (_currentPlayToImage != null)
            {
                _currentPlayToImage.PlayToSource.Next = newImageElement.PlayToSource;
                _currentPlayToImage.PlayToSource.PlayNext();
                _currentPlayToImage = newImageElement;
            }
            
            // Swap the new image into the UI preview area
            SelectedImageContainer.Content = newImageElement;
        }
    }
}
