namespace TapAndGoProximityNetworking
{
    using System;

    using Windows.UI.Core;
    using Windows.UI.Xaml;

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const string DownloadUri =
            "http://media.ch9.ms/ch9/6b47/afc378c0-2a0a-42f1-879b-16c0be966b47/TapAndGo_mid.mp4";

        private const string LocalName = "TapAndGo_Prosise.mp4";

        private CancellationTokenSource cts;

        private DownloadOperation download; 

        public MainPage()
        {
            cts = new CancellationTokenSource();
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.CheckState();
        }

        /// <summary>
        /// Main way to tell "where we are"
        /// </summary>
        /// <returns>Just a task to wait for</returns>
        private async Task CheckState()
        {
            try
            {
                var downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();

                if (downloads.Count < 1)
                {
                    var video = await CheckVideo();

                    // no downloads and the video is there 
                    if (video != null)
                    {
                        VisualStateManager.GoToState(this, "DownloadedPrevious", false);
                        return;
                    }                    
                }

                // downloads in progress 
                if (downloads.Count > 0)
                {
                    download = downloads[0];
                    this.UpdateFromProgress();
                    VisualStateManager.GoToState(this, "Downloading", false);
                    await this.DownloadProgressAsync(false);
                }
                else
                {
                    // not downloaded yet
                    VisualStateManager.GoToState(this, "NotDownloaded", false);
                }
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Checks to see if the video exists on disk and if so, returns it
        /// </summary>
        /// <returns>The video instance or null</returns>
        private static async Task<StorageFile> CheckVideo()
        {
            try
            {
                return await KnownFolders.VideosLibrary.GetFileAsync(LocalName);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Shows the video when it's been downloaded
        /// </summary>
        /// <returns>True if the video is successfully launched</returns>
        private async Task<bool> ShowVideo()
        {
            var video = await CheckVideo();

            if (video != null)
            {
                VisualStateManager.GoToState(this, "Downloaded", false);
                return await Windows.System.Launcher.LaunchFileAsync(video);                
            }

            return false;
        }

        /// <summary>
        /// Already downloaded so try to watch it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WatchOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(await this.ShowVideo()))
                {
                    throw new Exception("There was an unknown error attempting to launch the video file.");
                }

                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Already downloaded, user wants to delete it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var video = await CheckVideo();
                if (video != null)
                {
                    video.DeleteAsync();
                }
                VisualStateManager.GoToState(this, "NotDownloaded", false);
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Not downloaded, so user wishes to opt-in for a long-running download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = new Uri(DownloadUri, UriKind.Absolute);
                var destinationFile =
                    await KnownFolders.VideosLibrary.CreateFileAsync(LocalName, CreationCollisionOption.ReplaceExisting);

                this.ResumeButton.IsEnabled = false;

                var downloader = new BackgroundDownloader();
                download = downloader.CreateDownload(source, destinationFile);

                // start showing the progress
                this.UpdateFromProgress();
                VisualStateManager.GoToState(this, "Downloading", false);                    
                await DownloadProgressAsync(true);
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// When the user opts not to download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        /// <summary>
        /// Cancel the download (deletes the partial file)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CancelDownloadOnClick(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
            try
            {
                var folder = await KnownFolders.VideosLibrary.GetFileAsync(LocalName);
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (FileNotFoundException)
            {
                // do nothing
            }
            await this.CheckState();
        }

        /// <summary>
        /// Pause to resume later (may pause automatically if you lose connection or go on cellular)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseDownloadOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.download.Pause();
                this.PauseButton.IsEnabled = false;
                this.ResumeButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Resume when previously paused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResumeDownloadOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.download.Resume();
                this.PauseButton.IsEnabled = true;
                this.ResumeButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Update the progress - bytes downloaded, etc.
        /// </summary>
        private void UpdateFromProgress()
        {
            BytesReceived.Text = download.Progress.BytesReceived.ToString();
            TotalBytes.Text = download.Progress.TotalBytesToReceive.ToString();

            if (download.Progress.TotalBytesToReceive > 0)
            {
                ProgressIndicator.IsIndeterminate = false;
                ProgressIndicator.Maximum = download.Progress.TotalBytesToReceive;
                ProgressIndicator.Value = download.Progress.BytesReceived;
            }
            else
            {
                ProgressIndicator.IsIndeterminate = true;                
            }

            if (download.Progress.Status.Equals(BackgroundTransferStatus.PausedByApplication))
            {
                ResumeButton.IsEnabled =
                    download.Progress.Status.Equals(BackgroundTransferStatus.PausedByApplication);
                PauseButton.IsEnabled = !ResumeButton.IsEnabled;
            }

            if (download.Progress.Status.Equals(BackgroundTransferStatus.PausedCostedNetwork)
                || download.Progress.Status.Equals(BackgroundTransferStatus.PausedNoNetwork))
            {
                ResumeButton.IsEnabled = false;
            }            
        }

        /// <summary>
        /// Long running loop that starts the download or attaches to the existing download, task completes when done
        /// </summary>
        /// <param name="firstTime"></param>
        /// <returns></returns>
        private async Task DownloadProgressAsync(bool firstTime)
        {
            try
            {
                var progress = new Progress<DownloadOperation>(UpdateProgress);

                if (firstTime)
                {
                    await this.download.StartAsync().AsTask(cts.Token, progress);
                }
                else
                {
                    await this.download.AttachAsync().AsTask(cts.Token, progress);
                }

                this.download = null;
                this.cts.Dispose();
                this.cts = null;

                if (!(await this.ShowVideo()))
                {
                    throw new Exception("An unknown issue was encountered trying to launch the video.");
                }
                
                Application.Current.Exit();                
            }
            catch (Exception ex)
            {
                this.ProcessError(ex);
            }
        }

        /// <summary>
        /// Simply dispatches the progress to the UI thread
        /// </summary>
        /// <param name="obj"></param>
        private void UpdateProgress(DownloadOperation obj)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, this.UpdateFromProgress);
        }

        /// <summary>
        /// Go into panic mode
        /// </summary>
        /// <param name="ex"></param>
        private void ProcessError(Exception ex)
        {
            ErrorMessage.Text = ex.Message;
            VisualStateManager.GoToState(this, "Error", false);
        }
    }
}