using System;
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
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion


        private PlayToReceiver _receiver;
        private DisplayRequest _displayRequest;
        private CoreDispatcher _dispatcher;
        private Boolean _isSeeking = false;

        private async void HandleStartReceiverClicked(Object sender, RoutedEventArgs e)
        {
            try
            {
                _dispatcher = Window.Current.CoreWindow.Dispatcher;

                if (_receiver == null)
                {
                    _receiver = new PlayToReceiver();
                }

                // Add Play To Receiver events and properties
                _receiver.CurrentTimeChangeRequested += HandleReceiverCurrentTimeChangeRequested;
                _receiver.MuteChangeRequested += HandleReceiverMuteChangeRequested;
                _receiver.PauseRequested += HandleReceiverPauseRequested;
                _receiver.PlaybackRateChangeRequested += HandleReceiverPlaybackRateChangeRequested;
                _receiver.PlayRequested += HandleReceiverPlayRequested;
                _receiver.SourceChangeRequested += HandleReceiverSourceChangeRequested;
                _receiver.StopRequested += HandleReceiverStopRequested;
                _receiver.TimeUpdateRequested += HandleReceiverTimeUpdateRequested;
                _receiver.VolumeChangeRequested += HandleReceiverVolumeChangeRequested;

                _receiver.FriendlyName = "Example Play To Receiver";
                _receiver.SupportsAudio = false;
                _receiver.SupportsVideo = true;
                _receiver.SupportsImage = false;

                // Add MediaElement events
                VideoPlayer.CurrentStateChanged += HandleVideoPlayerCurrentStateChanged;
                VideoPlayer.MediaEnded += HandleVideoPlayerMediaEnded;
                VideoPlayer.MediaFailed += HandleVideoPlayerMediaFailed;
                VideoPlayer.MediaOpened += HandleVideoPlayerMediaOpened;
                VideoPlayer.RateChanged += HandleVideoPlayerRateChanged;
                VideoPlayer.SeekCompleted += HandleVideoPlayerSeekCompleted;
                VideoPlayer.VolumeChanged += HandleVideoPlayerVolumeChanged;

                // Advertise the receiver on the local network and start receiving commands
                await _receiver.StartAsync();

                // Prevent the screen from locking
                if (_displayRequest == null)
                {
                    _displayRequest = new DisplayRequest();
                }
                _displayRequest.RequestActive();

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

                    if (_displayRequest != null)
                    {
                        _displayRequest.RequestRelease();
                    }

                    // Remove Play To Receiver events
                    _receiver.CurrentTimeChangeRequested -= HandleReceiverCurrentTimeChangeRequested;
                    _receiver.MuteChangeRequested -= HandleReceiverMuteChangeRequested;
                    _receiver.PauseRequested -= HandleReceiverPauseRequested;
                    _receiver.PlaybackRateChangeRequested -= HandleReceiverPlaybackRateChangeRequested;
                    _receiver.PlayRequested -= HandleReceiverPlayRequested;
                    _receiver.SourceChangeRequested -= HandleReceiverSourceChangeRequested;
                    _receiver.StopRequested -= HandleReceiverStopRequested;
                    _receiver.TimeUpdateRequested -= HandleReceiverTimeUpdateRequested;
                    _receiver.VolumeChangeRequested -= HandleReceiverVolumeChangeRequested;

                    //  Remove MediaElement events
                    VideoPlayer.Pause();

                    VideoPlayer.CurrentStateChanged -= HandleVideoPlayerCurrentStateChanged;
                    VideoPlayer.MediaEnded -= HandleVideoPlayerMediaEnded;
                    VideoPlayer.MediaFailed -= HandleVideoPlayerMediaFailed;
                    VideoPlayer.MediaOpened -= HandleVideoPlayerMediaOpened;
                    VideoPlayer.RateChanged -= HandleVideoPlayerRateChanged;
                    VideoPlayer.SeekCompleted -= HandleVideoPlayerSeekCompleted;
                    VideoPlayer.VolumeChanged -= HandleVideoPlayerVolumeChanged;

                    StatusText.Text = "Stopped '" + _receiver.FriendlyName + "'.";
                }
            }
            catch
            {
                var friendlyName = _receiver == null ? String.Empty : _receiver.FriendlyName;
                StatusText.Text = "Failed to stop '" + friendlyName + "'.";
            }
        }

        private async void HandleReceiverCurrentTimeChangeRequested(PlayToReceiver sender, CurrentTimeChangeRequestedEventArgs args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.Position = args.Time;
                    _receiver.NotifySeeking();
                    _isSeeking = true;
                });
        }

        private async void HandleReceiverMuteChangeRequested(PlayToReceiver sender, MuteChangeRequestedEventArgs args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.IsMuted = args.Mute;
                });
        }

        private async void HandleReceiverPauseRequested(PlayToReceiver sender, Object args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.Pause();
                });
        }

        private async void HandleReceiverPlaybackRateChangeRequested(PlayToReceiver sender, PlaybackRateChangeRequestedEventArgs args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.PlaybackRate = args.Rate;
                });
        }

        private async void HandleReceiverPlayRequested(PlayToReceiver sender, Object args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.Play();
                });
        }

        private async void HandleReceiverSourceChangeRequested(PlayToReceiver sender, SourceChangeRequestedEventArgs args)
        {
            if (args.Stream != null)
                await _dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {
                        var stream = args.Stream as Windows.Storage.Streams.IRandomAccessStream;
                        VideoPlayer.SetSource(stream, args.Stream.ContentType);
                    });
        }

        private async void HandleReceiverStopRequested(PlayToReceiver sender, Object args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.Stop();
                });
        }

        private async void HandleReceiverTimeUpdateRequested(PlayToReceiver sender, Object args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    if (VideoPlayer.Position != null)
                    {
                        _receiver.NotifyTimeUpdate(VideoPlayer.Position);
                    }
                });
        }

        private async void HandleReceiverVolumeChangeRequested(PlayToReceiver sender, VolumeChangeRequestedEventArgs args)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    VideoPlayer.Volume = args.Volume;
                });
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

        private void HandleVideoPlayerMediaFailed(Object sender, ExceptionRoutedEventArgs e)
        {
            if (_receiver != null) { _receiver.NotifyError(); }
        }

        private void HandleVideoPlayerMediaEnded(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyEnded();
                VideoPlayer.Stop();
            }
        }

        private void HandleVideoPlayerMediaOpened(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyDurationChange(VideoPlayer.NaturalDuration.TimeSpan);
                _receiver.NotifyLoadedMetadata();
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

        private void HandleVideoPlayerVolumeChanged(Object sender, RoutedEventArgs e)
        {
            if (_receiver != null)
            {
                _receiver.NotifyVolumeChange(VideoPlayer.Volume, VideoPlayer.IsMuted);
            }
        }
    }
}
