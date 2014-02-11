using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using MultimediaExample.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MultimediaExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Fields

        private readonly NavigationHelper _navigationHelper;
        private readonly PlaybackViewModel _defaultViewModel;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            _defaultViewModel = new PlaybackViewModel(IsFileSupported, x => Frame.Navigate(x));

            InitializeComponent();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public PlaybackViewModel DefaultViewModel
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);

            DefaultViewModel.CurrentPlaybackFileChanged += HandleCurrentPlaybackFileChanged;

            PlaybackWindow.MediaOpened += (sender, args) => Debug.WriteLine("Media Opened");
            PlaybackWindow.MediaFailed += (sender, args) => Debug.WriteLine("Media Failed");
            PlaybackWindow.MediaEnded += HandlePlaybackWindowMediaEnded;

            PlaybackWindow.VolumeChanged += (sender, args) => Debug.WriteLine("Volume Changed");


            PlaybackWindow.MarkerReached += HandlePlaybackWindowMarkerReached;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        
        // Frame.Navigate(typeof (CameraCapturePage));

        private async void HandleCurrentPlaybackFileChanged(Object sender, EventArgs e)
        {
            var currentFile = DefaultViewModel.CurrentPlaybackFile;
            if (currentFile != null)
            {
                var stream = await currentFile.OpenReadAsync();
                PlaybackWindow.SetSource(stream, currentFile.ContentType);
                //PlaybackWindow.SetMediaStreamSource();
                //PlaybackWindow.SetSource(IRandomAccessStream, mimeType);
                //PlaybackWindow.Source = Uri
            }
            
            // Buffering and Download
            //PlaybackWindow.DownloadProgress
            //PlaybackWindow.DownloadProgressChanged
            //PlaybackWindow.DownloadProgressOffset (relates to seek-ahead)
            //PlaybackWindow.BufferingProgress (0-1)...percentage

            // Markers
            //PlaybackWindow.Markers --> TimelineMarkerCollection
            //PlaybackWindow.MarkerReached

            // PlayTo
            //PlaybackWindow.PlayToPreferredSourceUri
            //PlaybackWindow.PlayToSource

            // Effects          
            //PlaybackWindow.AddAudioEffect();
            //PlaybackWindow.AddVideoEffect();
            //PlaybackWindow.RemoveAllEffects();

            // Other?
            //PlaybackWindow.RealTimePlayback
            //PlaybackWindow.ProtectionManager

        }

        private void HandlePlaybackWindowMediaEnded(Object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.WriteLine(PlaybackWindow.Source);
            var currentIndex = DefaultViewModel.PlaybackFiles.IndexOf(DefaultViewModel.CurrentPlaybackFile);
            var newIndex = currentIndex + 1;
            if (newIndex < DefaultViewModel.PlaybackFiles.Count())
            {
                var newFile = DefaultViewModel.PlaybackFiles[newIndex];
                DefaultViewModel.CurrentPlaybackFile = newFile;
            }
        }

        private void HandlePlaybackWindowMarkerReached(Object sender, TimelineMarkerRoutedEventArgs e)
        {
            Debug.WriteLine("Marker reached: {0}", e.Marker.Text);
        }

        private void HandlePlayClicked(Object sender, RoutedEventArgs e)
        {
            PlaybackWindow.Play();
        }

        private void HandlePauseClicked(Object sender, RoutedEventArgs e)
        {
            PlaybackWindow.Pause();
        }

        private void HandleStopClicked(Object sender, RoutedEventArgs e)
        {
            PlaybackWindow.Stop();
        }

        private void HandleBack5Clicked(Object sender, RoutedEventArgs e)
        {
            SeekToPosition(TimeSpan.FromSeconds(-5));
        }

        private void HandleForward5Clicked(Object sender, RoutedEventArgs e)
        {
            SeekToPosition(TimeSpan.FromSeconds(5));
        }

        private void SeekToPosition(TimeSpan newPosition)
        {
            //PlaybackWindow.SeekCompleted
            
            // Make sure that seek is an option
            if (!PlaybackWindow.CanSeek) return;

            // Pause any current playback
            if (PlaybackWindow.CanPause) PlaybackWindow.Pause();

            // Determine the new position
            var newPos = PlaybackWindow.Position + newPosition;

            // Make sure the new position is "in bounds"
            if (newPos < TimeSpan.FromMilliseconds(0))
            {
                newPos = TimeSpan.FromMilliseconds(0);
            }

            // Note that NaturalDuration is "Automatic" until after MediaOpened event is raised
            var duration = PlaybackWindow.NaturalDuration;
            if (duration.HasTimeSpan)
            {
                if (newPos > duration.TimeSpan) newPos = duration.TimeSpan;
            }

            // Finally, set the target position
            PlaybackWindow.Position = newPos;
        }

        private void HandleSloMoCheckChanged(Object sender, RoutedEventArgs e)
        {
            if (PlaybackWindow.CanPause) PlaybackWindow.Pause();

            var checkBox = (CheckBox) sender;
            var playbackRate = checkBox.IsChecked == true ? 0.5 : 1.0;
            PlaybackWindow.DefaultPlaybackRate = playbackRate;

            //PlaybackWindow.PlaybackRate
            //PlaybackWindow.RateChanged
        }

        private Boolean IsFileSupported(String fileType)
        {
            return PlaybackWindow.CanPlayType(fileType) != MediaCanPlayResponse.NotSupported;
        }
    }

    public class EnumTextValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var mediaElementStateValue = (Enum) value;
            return mediaElementStateValue.ToString();
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
