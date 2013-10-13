using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.PlayTo;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PlayToExample.Common;

namespace PlayToExample
{
    /// <summary>
    /// The page to stream video to a selected PlayTo device.
    /// </summary>
    public sealed partial class VideoPage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoPage"/> class.
        /// </summary>
        public VideoPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlayToManager.GetForCurrentView().SourceRequested += OnPlayToSourceRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PlayToManager.GetForCurrentView().SourceRequested -= OnPlayToSourceRequested;
        }

        private void OnPlayToSourceRequested(PlayToManager sender, 
            PlayToSourceRequestedEventArgs args)
        {
            // This request will come in on a non-UI thread, 
            // so it will need to be marshalled over.  
            // Since doing that is an async operation, 
            // a deferral will be required.
            var deferral = args.SourceRequest.GetDeferral();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MediaElement.PlayToSource == null)
                {
                    var errorMessage = "There is no video selected to be streamed.";
                    args.SourceRequest.DisplayErrorString(errorMessage);
                }
                else
                {
                    args.SourceRequest.SetSource(MediaElement.PlayToSource);
                }
                   
                deferral.Complete();
            });
        }

        private async void HandleUseWebCamClicked(Object sender, RoutedEventArgs e)
        {
            // Capture the file from a webcam and then play it back
            var cameraCapture = new CameraCaptureUI();
            var fileSource = await cameraCapture.CaptureFileAsync(CameraCaptureUIMode.Video);
            await SetMediaSource(fileSource);
        }

        private async void HandleSelectFileClicked(Object sender, RoutedEventArgs e)
        {
            // Select the file from the "file system" (uses the picker, so it could be anywhere...) and play it back
            var filePicker = new FileOpenPicker
                             {
                                 SuggestedStartLocation = PickerLocationId.VideosLibrary,
                                 FileTypeFilter = {".mp4", ".wmv"},
                                 ViewMode = PickerViewMode.Thumbnail,
                             };

            var fileSource = await filePicker.PickSingleFileAsync();
            await SetMediaSource(fileSource);
        }

        private async Task SetMediaSource(StorageFile fileSource)
        {
            if (fileSource != null)
            {
                var mediaStream = await fileSource.OpenAsync(FileAccessMode.Read);
                MediaElement.SetSource(mediaStream, fileSource.ContentType);
            }
        }
    }
}
