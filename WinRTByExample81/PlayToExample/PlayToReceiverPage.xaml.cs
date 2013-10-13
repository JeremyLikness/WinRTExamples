using System;
using Windows.Media.PlayTo;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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

        private enum PlaybackType
        {
            None,
            Video,
            Image
        }

        private PlaybackType _currentPlaybackType;

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
                _receiver.SupportsAudio = true;
                _receiver.SupportsVideo = true;
                _receiver.SupportsImage = true;

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

        private BitmapImage _imageSource;

        private void HandleReceiverSourceChangeRequested(PlayToReceiver sender, SourceChangeRequestedEventArgs args)
        {
            if (args.Stream != null)
            {
                if (args.Stream.ContentType.Contains("image"))
                {
                    Dispatch(() =>
                    {
                        _imageSource = new BitmapImage();
                        RoutedEventHandler handler = null;
                        handler = (o, eventArgs) =>
                        {
                            _receiver.NotifyLoadedMetadata();
                            _imageSource.ImageOpened -= handler;
                        };
                        _imageSource.ImageOpened += handler;
                        _imageSource.SetSource(args.Stream);

                        if (_currentPlaybackType != PlaybackType.Image)
                        {
                            if (_currentPlaybackType == PlaybackType.Video)
                            {
                                VideoPlayer.Stop();
                            }

                            ImagePlayer.Opacity = 1;
                            VideoPlayer.Opacity = 0;
                        }
                        _currentPlaybackType = PlaybackType.Image;
                    });
                }
                else
                {
                    Dispatch(() =>
                    {
                        VideoPlayer.SetSource(args.Stream, args.Stream.ContentType);
                        if (_currentPlaybackType != PlaybackType.Video)
                        {
                            if (_currentPlaybackType == PlaybackType.Image)
                            {
                                ImagePlayer.Source = null;
                            }

                            ImagePlayer.Opacity = 0;
                            VideoPlayer.Opacity = 1;
                        }
                        _currentPlaybackType = PlaybackType.Video;
                    });
                }
            }
        }

        private void HandleReceiverPlayRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() =>
            {
                if (_currentPlaybackType == PlaybackType.Video)
                {
                    VideoPlayer.Play();
                }
                else if (_currentPlaybackType == PlaybackType.Image)
                {
                    ImagePlayer.Source = _imageSource;
                    _receiver.NotifyPlaying();
                }
            });
        }

        private void HandleReceiverPauseRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() => VideoPlayer.Pause());
        }

        private void HandleReceiverStopRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() =>
            {
                if (_currentPlaybackType == PlaybackType.Video)
                {
                    VideoPlayer.Stop();
                }
                else if (_currentPlaybackType == PlaybackType.Image)
                {
                    ImagePlayer.Source = null;
                }
            });
        }

        private void HandleReceiverPlaybackRateChangeRequested(PlayToReceiver sender, PlaybackRateChangeRequestedEventArgs args)
        {
            Dispatch(() => { VideoPlayer.PlaybackRate = args.Rate; });
        }


        private void HandleReceiverCurrentTimeChangeRequested(PlayToReceiver sender, CurrentTimeChangeRequestedEventArgs args)
        {
            Dispatch(() =>
                     {
                         if (_currentPlaybackType == PlaybackType.Video)
                         {
                             VideoPlayer.Position = args.Time;
                             _receiver.NotifySeeking();
                             _isSeeking = true;
                         }
                         else if (_currentPlaybackType == PlaybackType.Image)
                         {
                             _receiver.NotifySeeking();
                             _receiver.NotifySeeked();
                         }
                     });
        }

        private void HandleReceiverTimeUpdateRequested(PlayToReceiver sender, Object args)
        {
            Dispatch(() =>
            {
                if (_currentPlaybackType == PlaybackType.Video)
                {
                    _receiver.NotifyTimeUpdate(VideoPlayer.Position);
                }
                else if (_currentPlaybackType == PlaybackType.Image)
                {
                    _receiver.NotifyTimeUpdate(new TimeSpan(0));
                }
            });
        }

        private void HandleReceiverVolumeChangeRequested(PlayToReceiver sender, VolumeChangeRequestedEventArgs args)
        {
            Dispatch(() => { VideoPlayer.Volume = args.Volume; });
        }

        private void HandleReceiverMuteChangeRequested(PlayToReceiver sender, MuteChangeRequestedEventArgs args)
        {
            Dispatch(() =>
            {
                if (_currentPlaybackType == PlaybackType.Video)
                {
                    VideoPlayer.IsMuted = args.Mute;
                }
                else if (_currentPlaybackType == PlaybackType.Image)
                {
                    _receiver.NotifyVolumeChange(0, args.Mute);
                }
            });
        }

        private async void Dispatch(Action dispatchAction)
        {
            // Utility function to do a standard cross-thread dispatch
            if (dispatchAction == null) throw new ArgumentNullException("dispatchAction");
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, dispatchAction.Invoke);
        } 
        #endregion


        #region Video Player Event Handling
        // Receive the request from the MediaElement and map it to how it should be handled in the source

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

        private void HandleImagePlayerImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (_receiver != null) { _receiver.NotifyError(); }
        }
    }
}
