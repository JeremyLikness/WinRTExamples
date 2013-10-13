using System;
using System.Threading.Tasks;
using Windows.Media.PlayTo;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlayToExample.Common;

namespace PlayToExample
{
    /// <summary>
    /// The page to demonstrate being a Play To Receiver
    /// </summary>
    public sealed partial class PlayToReceiverPage : Page
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
        /// Initializes a new instance of the <see cref="PlayToReceiverPage"/> class.
        /// </summary>
        public PlayToReceiverPage()
        {
            InitializeComponent();
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
        private void navigationHelper_LoadState(Object sender, LoadStateEventArgs e)
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_displayRequest != null)
            {
                _displayRequest.RequestRelease();
                _displayRequest = null;
            }
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private PlayToReceiver _receiver;
        private DisplayRequest _displayRequest;
        private Boolean _isSeeking = false;

        private async void HandleStartReceiverClicked(Object sender, RoutedEventArgs e)
        {
            try
            {
                if (_receiver == null)
                {
                    _receiver = new PlayToReceiver();
                }

                // Set the Properties that describe this receiver device to other systems
                _receiver.FriendlyName = "Example Play To Receiver";
                _receiver.SupportsAudio = false;
                _receiver.SupportsVideo = true;
                _receiver.SupportsImage = false;

                // Subscribe to Play To Receiver events
                // Receive the request from the Play To source and map it to how it should be handled in this app
                // (Marshall the call and tell the MediaElement to respond in-kind.)
                _receiver.SourceChangeRequested += HandleReceiverSourceChangeRequested; 

                // Playback commands
                _receiver.PlayRequested += HandleReceiverPlayRequested;
                _receiver.PauseRequested += HandleReceiverPauseRequested;
                _receiver.StopRequested += HandleReceiverStopRequested; 
                _receiver.PlaybackRateChangeRequested += HandleReceiverPlaybackRateChangeRequested;

                // Seek commands
                _receiver.CurrentTimeChangeRequested += HandleReceiverCurrentTimeChangeRequested;
                _receiver.TimeUpdateRequested += HandleReceiverTimeUpdateRequested;
                
                // Volume commands
                _receiver.VolumeChangeRequested += HandleReceiverVolumeChangeRequested;
                _receiver.MuteChangeRequested += HandleReceiverMuteChangeRequested;

                // Subscribe to MediaElement events
                // Receive the request from the MediaElement and map it to how it should be handled in the source
                VideoPlayer.MediaOpened += HandleVideoPlayerMediaOpened;
                VideoPlayer.CurrentStateChanged += HandleVideoPlayerCurrentStateChanged;
                VideoPlayer.RateChanged += HandleVideoPlayerRateChanged;
                VideoPlayer.SeekCompleted += HandleVideoPlayerSeekCompleted;
                VideoPlayer.MediaEnded += HandleVideoPlayerMediaEnded;
                VideoPlayer.VolumeChanged += HandleVideoPlayerVolumeChanged; 
                VideoPlayer.MediaFailed += HandleVideoPlayerMediaFailed;
               
                // Advertise the receiver on the local network and start receiving commands
                await _receiver.StartAsync();

                // Use the DisplayRequest to prevent power-save from interrupting the playback experience
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                    _displayRequest.RequestActive();
                }

                StatusText.Text = "'" + _receiver.FriendlyName + "' started.";
            }
            catch
            {
                _receiver = null;
                StatusText.Text = "Failed to start receiver.";
            }

        }

        private async void HandleStopReceiverClicked(Object sender, RoutedEventArgs e)
        {
            try
            {
                if (_receiver != null)
                {
                    await _receiver.StopAsync();

                    // The DisplayRequest should be released as soon as it is not needed anymore
                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                        _displayRequest = null;
                    }

                    // Remove Play To Receiver events
                    _receiver.SourceChangeRequested -= HandleReceiverSourceChangeRequested;
                    _receiver.PlayRequested -= HandleReceiverPlayRequested;
                    _receiver.PauseRequested -= HandleReceiverPauseRequested;
                    _receiver.StopRequested -= HandleReceiverStopRequested;
                    _receiver.PlaybackRateChangeRequested -= HandleReceiverPlaybackRateChangeRequested; 
                    _receiver.CurrentTimeChangeRequested -= HandleReceiverCurrentTimeChangeRequested;
                    _receiver.TimeUpdateRequested -= HandleReceiverTimeUpdateRequested;
                    _receiver.MuteChangeRequested -= HandleReceiverMuteChangeRequested; 
                    _receiver.VolumeChangeRequested -= HandleReceiverVolumeChangeRequested;

                    //  Remove MediaElement events
                    VideoPlayer.Pause();

                    VideoPlayer.MediaOpened -= HandleVideoPlayerMediaOpened;
                    VideoPlayer.CurrentStateChanged -= HandleVideoPlayerCurrentStateChanged;
                    VideoPlayer.RateChanged -= HandleVideoPlayerRateChanged;
                    VideoPlayer.SeekCompleted -= HandleVideoPlayerSeekCompleted; 
                    VideoPlayer.MediaEnded -= HandleVideoPlayerMediaEnded;
                    VideoPlayer.VolumeChanged -= HandleVideoPlayerVolumeChanged;
                    VideoPlayer.MediaFailed -= HandleVideoPlayerMediaFailed;

                    StatusText.Text = "Stopped '" + _receiver.FriendlyName + "'.";
                }
            }
            catch
            {
                var friendlyName = _receiver == null ? String.Empty : _receiver.FriendlyName;
                StatusText.Text = "Failed to stop '" + friendlyName + "'.";
            }
        }

        #region Play To Receiver Event Mappings

        private void HandleReceiverSourceChangeRequested(PlayToReceiver sender, SourceChangeRequestedEventArgs args)
        {
            if (args.Stream != null)
            {
                Dispatch(() => VideoPlayer.SetSource(args.Stream, args.Stream.ContentType));
            }
        }

        private void HandleReceiverPlayRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() => VideoPlayer.Play());
        }

        private void HandleReceiverPauseRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() => VideoPlayer.Pause());
        }

        private void HandleReceiverStopRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() => VideoPlayer.Stop());
        }

        private void HandleReceiverPlaybackRateChangeRequested(PlayToReceiver sender, PlaybackRateChangeRequestedEventArgs args)
        {
            Dispatch(() => { VideoPlayer.PlaybackRate = args.Rate; });
        }


        private void HandleReceiverCurrentTimeChangeRequested(PlayToReceiver sender, CurrentTimeChangeRequestedEventArgs args)
        {
            Dispatch(() =>
                     {
                         VideoPlayer.Position = args.Time;
                         _receiver.NotifySeeking();
                         _isSeeking = true;
                     });
        }

        private void HandleReceiverTimeUpdateRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() => _receiver.NotifyTimeUpdate(VideoPlayer.Position));
        }

        private void HandleReceiverVolumeChangeRequested(PlayToReceiver sender, VolumeChangeRequestedEventArgs args)
        {
            Dispatch(() => { VideoPlayer.Volume = args.Volume; });
        }

        private void HandleReceiverMuteChangeRequested(PlayToReceiver sender, MuteChangeRequestedEventArgs args)
        {
            Dispatch(() => { VideoPlayer.IsMuted = args.Mute; });
        }

        private async void Dispatch(Action dispatchAction)
        {
            // Utility function to do a standard cross-thread dispatch
            if (dispatchAction == null) throw new ArgumentNullException("dispatchAction");
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, dispatchAction.Invoke);
        } 
        #endregion


        #region Video Player Event Handling

        private void HandleVideoPlayerMediaOpened(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyDurationChange(VideoPlayer.NaturalDuration.TimeSpan);
                _receiver.NotifyLoadedMetadata();
            }
        }

        private void HandleVideoPlayerCurrentStateChanged(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                switch (VideoPlayer.CurrentState)
                {
                    case MediaElementState.Playing:
                        _receiver.NotifyPlaying();
                        break;
                    case MediaElementState.Paused:
                        _receiver.NotifyPaused();
                        break;
                    case MediaElementState.Stopped:
                        _receiver.NotifyStopped();
                        break;
                }
            }
        }

        private void HandleVideoPlayerRateChanged(Object sender, RateChangedRoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyRateChange(VideoPlayer.PlaybackRate);
            }
        }

        private void HandleVideoPlayerSeekCompleted(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                if (!_isSeeking)
                {
                    _receiver.NotifySeeking();
                }
                _receiver.NotifySeeked();
                _isSeeking = false;
            }
        }

        private void HandleVideoPlayerMediaEnded(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyEnded();
                VideoPlayer.Stop();
            }
        }

        private void HandleVideoPlayerVolumeChanged(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyVolumeChange(VideoPlayer.Volume, VideoPlayer.IsMuted);
            }
        }

        private void HandleVideoPlayerMediaFailed(Object sender, ExceptionRoutedEventArgs e)
        {
            if (_receiver != null) { _receiver.NotifyError(); }
        } 
        #endregion
    }
}
