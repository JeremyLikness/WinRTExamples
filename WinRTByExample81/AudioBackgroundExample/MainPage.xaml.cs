using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AudioBackgroundExample
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Windows.Media;
    using Windows.Storage;
    using Windows.Storage.Pickers;
    using Windows.UI.Core;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private string fileName = "No file selected";

        private SystemMediaTransportControls transportControls;

        private readonly CoreDispatcher dispatcher;

        private readonly Dictionary<MediaElementState, MediaPlaybackStatus> map =
            new Dictionary<MediaElementState, MediaPlaybackStatus>
                {
                    {
                        MediaElementState.Buffering,
                        MediaPlaybackStatus.Changing
                    },
                    {
                        MediaElementState.Closed,
                        MediaPlaybackStatus.Closed
                    },
                    {
                        MediaElementState.Opening,
                        MediaPlaybackStatus.Changing
                    },
                    {
                        MediaElementState.Paused,
                        MediaPlaybackStatus.Paused
                    },
                    {
                        MediaElementState.Playing,
                        MediaPlaybackStatus.Playing
                    },
                    {
                        MediaElementState.Stopped,
                        MediaPlaybackStatus.Stopped
                    }
                };

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += this.MainPageLoaded;
            this.dispatcher = this.Dispatcher;
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            this.transportControls = SystemMediaTransportControls.GetForCurrentView();
            this.transportControls.ButtonPressed += this.TransportControlsButtonPressed;            
            MediaPlayer.AudioCategory = AudioCategory.BackgroundCapableMedia;
            MediaPlayer.AreTransportControlsEnabled = false;
            MediaPlayer.AutoPlay = false;
            MediaPlayer.CurrentStateChanged += (o, args) =>
                {
                    this.SetButtonStates();
                    this.SetTransportControlStates();
                };
            this.SetButtonStates();
        }

        private async void TransportControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button.Equals(SystemMediaTransportControlsButton.Play))
            {
                await RunOnUiThread(this.Play);
                return;
            }

            if (args.Button.Equals(SystemMediaTransportControlsButton.Pause))
            {
                await RunOnUiThread(this.Pause);
                return;
            }

            if (args.Button.Equals(SystemMediaTransportControlsButton.Stop))
            {
                await RunOnUiThread(this.Stop);                
            }
        }

        private async Task RunOnUiThread(Action action)
        {
            await this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        private async void SelectFileOnClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            var file = await picker.PickSingleFileAsync();
            
            if (file == null)
            {
                return;
            }
            
            try
            {
                await this.transportControls.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, file);
                this.MediaPlayer.SetSource(await file.OpenAsync(FileAccessMode.Read), file.ContentType);
                this.transportControls.DisplayUpdater.Update();
                this.fileName = file.DisplayName;  
                this.SetTransportControlStates();
            }
            catch (Exception ex)
            {
                this.Status.Text = ex.Message;
            }
        }

        private void PlayOnClick(object sender, RoutedEventArgs e)
        {
            this.Play();
            this.SetTransportControlStates();
        }

        private void PauseOnClick(object sender, RoutedEventArgs e)
        {
            this.Pause();
            this.SetTransportControlStates();
        }

        private void StopOnClick(object sender, RoutedEventArgs e)
        {
            this.Stop();
            this.SetTransportControlStates();
        }

        private void SetButtonStates()
        {
            Status.Text = string.Format("{0}: {1}", this.fileName, MediaPlayer.CurrentState);
            PlayButton.IsEnabled = MediaPlayer.CurrentState == MediaElementState.Paused || MediaPlayer.CurrentState == MediaElementState.Stopped;
            PauseButton.IsEnabled = MediaPlayer.CanPause && MediaPlayer.CurrentState == MediaElementState.Playing;
            StopButton.IsEnabled = MediaPlayer.CurrentState == MediaElementState.Playing;
        }

        private void SetTransportControlStates()
        {
            var validMusic = PlayButton.IsEnabled || PauseButton.IsEnabled || StopButton.IsEnabled;
            this.transportControls.IsEnabled = validMusic;
            this.transportControls.IsPlayEnabled = validMusic;
            this.transportControls.IsPauseEnabled = MediaPlayer.CanPause;
            this.transportControls.IsStopEnabled = validMusic;
            this.transportControls.PlaybackStatus = this.map[MediaPlayer.CurrentState];
        }

        private void Play()
        {
            try
            {
                MediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Status.Text = ex.Message;
            }
        }

        private void Pause()
        {
            if (MediaPlayer.CanPause && MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                MediaPlayer.Pause();
            }
        }

        private void Stop()
        {
            if (MediaPlayer.CurrentState == MediaElementState.Playing)
            {
                MediaPlayer.Stop();
            }
        }
    }
}
