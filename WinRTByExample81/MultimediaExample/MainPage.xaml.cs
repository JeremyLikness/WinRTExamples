using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using MultimediaExample.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
            var playbackWindowProxy = new PlaybackWindowProxy();
            _defaultViewModel = new PlaybackViewModel(playbackWindowProxy, x => Frame.Navigate(x));

            InitializeComponent();

            // Wait to pass the control until after the control has been created
            playbackWindowProxy.Initialize(PlaybackWindow);

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
        private void navigationHelper_SaveState(Object sender, SaveStateEventArgs e)
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

        #region Playback events

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
            PlaybackWindow.Pause();
            var matchingMarker = DefaultViewModel.CurrentPlaybackFile.FileMarkers.FirstOrDefault(x => x.Time == e.Marker.Time);
            DefaultViewModel.CurrentFileMarker = matchingMarker;
            TextToSpeechHelper.PlayContentAsync(matchingMarker.TextToSpeechContent, matchingMarker.IsSsml, matchingMarker.SelectedVoiceId);
        } 

        #endregion

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

        private void HandleAddMarkerClicked(Object sender, RoutedEventArgs e)
        {
            DefaultViewModel.PauseCommand.Execute(null);

            // Show a flyout that displays the current marker details
            var flyout = (Flyout)FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var flyoutContent = (FrameworkElement)flyout.Content;

            var newFileMarker = new FileMarker
            {
                Time = DefaultViewModel.GetCurrentPlaybackPosition()
            };

            var viewModel = new FileMarkerViewModel(DefaultViewModel, newFileMarker);

            flyoutContent.DataContext = viewModel;
            flyout.ShowAt((FrameworkElement)sender);
        }

        private void HandleUpdateMarkerClicked(Object sender, RoutedEventArgs e)
        {
            var marker = DefaultViewModel.CurrentFileMarker;

            // Show a flyout that displays the current marker details
            var flyout = (Flyout)FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var flyoutContent = (FrameworkElement)flyout.Content;

            var viewModel = new FileMarkerViewModel(DefaultViewModel, marker);

            flyoutContent.DataContext = viewModel;
            flyout.ShowAt((FrameworkElement)sender);
        }
    }

    public class DesignFileMarkerViewModel : FileMarkerViewModel
    {
        public DesignFileMarkerViewModel()
            : base(new PlaybackViewModel(null, null), new FileMarker())
        {
        }
    }
}
