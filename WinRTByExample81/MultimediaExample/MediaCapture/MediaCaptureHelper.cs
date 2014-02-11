using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MultimediaExample
{
    public class MediaCaptureHelper
    {
        #region Fields

        private MediaCapture _captureManager;
        private DeviceInformation _videoDeviceToUse;
        private DeviceInformation _audioDeviceToUse;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCaptureHelper"/> class.
        /// </summary>
        public MediaCaptureHelper()
        {
            // Skip the HW and lifetime related actions when in design mode
            if (DesignMode.DesignModeEnabled) return;

            Application.Current.Resuming += (o, e) =>
            {
                // Reset/restart on resume
                ApplySettings();
            };

            Application.Current.Suspending += async (o, e) =>
            {
                if (_captureManager == null) return;

                // Stop previewing when the app is being moved from the foreground
                var deferral = e.SuspendingOperation.GetDeferral();
                await _captureManager.StopPreviewAsync();
                _captureManager = null;
                deferral.Complete();
            };
        }

        #endregion

        public async Task<IEnumerable<DeviceInformation>> GetVideoCaptureDevicesAsync()
        {
            return (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).ToList();
        }

        public async Task<IEnumerable<DeviceInformation>> GetAudioCaptureDevicesAsync()
        {
            return (await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture)).ToList();
        }

        public DeviceInformation VideoDeviceToUse
        {
            get { return _videoDeviceToUse; }
            set
            {
                _videoDeviceToUse = value;
                ApplySettings();
            }
        }

        public DeviceInformation AudioDeviceToUse
        {
            get { return _audioDeviceToUse; }
            set
            {
                _audioDeviceToUse = value;
                ApplySettings();
            }
        }

        public IEnumerable<VideoEncodingQuality> VideoQualities
        {
            get { return Enum.GetValues(typeof(VideoEncodingQuality)).Cast<VideoEncodingQuality>(); }
        }

        public VideoEncodingQuality VideoQualityToUse { get; set; }

        public event EventHandler CaptureHasBeenReset;

        private void OnCaptureHasBeenReset()
        {
            var handler = CaptureHasBeenReset;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public async void StartCapturePreview(CaptureElement captureUIElement)
        {
            if (captureUIElement == null) throw new ArgumentNullException("captureUIElement");

            captureUIElement.Source = _captureManager;
            await _captureManager.StartPreviewAsync();
        }

        private const String WindowsMediaExtension = ".wmv";
        private const String MP4Extension = ".mp4";

        public async Task<MediaCaptureJob> StartCaptureAsync()
        {
            if (_captureManager == null) throw new InvalidOperationException("Capture Manager has not been initialized.");

            // Save type options
            var wmvFileSaveType = new KeyValuePair<String, IList<String>>("Windows Media", new List<String> { WindowsMediaExtension });
            var mp4FileSaveType = new KeyValuePair<String, IList<String>>("MP4", new List<String> { ".mp4" });

            // Get the file to save
            var savePicker = new FileSavePicker
                             {
                                 SuggestedStartLocation = PickerLocationId.VideosLibrary,
                                 SuggestedFileName = "Video Capture"
                             };
            savePicker.FileTypeChoices.Add(wmvFileSaveType);
            savePicker.FileTypeChoices.Add(mp4FileSaveType);
            var fileToSaveTo = await savePicker.PickSaveFileAsync();
            if (fileToSaveTo == null) return null;

            var captureJob = new MediaCaptureJob(_captureManager, fileToSaveTo);
            MediaEncodingProfile profile;
            switch (fileToSaveTo.FileType)
            {
                case WindowsMediaExtension:
                    profile = MediaEncodingProfile.CreateWmv(VideoQualityToUse);
                    break;
                case MP4Extension:
                    profile = MediaEncodingProfile.CreateMp4(VideoQualityToUse);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file type selected");
            }

            captureJob.StartCaptureAsync(profile);
            return captureJob;
        }

        public void ShowSettings()
        {
            if (_captureManager == null) throw new InvalidOperationException("Capture Manager has not been initialized.");

            // Display capture options UI
            CameraOptionsUI.Show(_captureManager); 
        }

        private async void ApplySettings()
        {
            var captureMode = StreamingCaptureMode.AudioAndVideo;
            if (VideoDeviceToUse == null && AudioDeviceToUse == null) return;
            if (VideoDeviceToUse != null && AudioDeviceToUse == null) captureMode = StreamingCaptureMode.Video;
            if (VideoDeviceToUse == null && AudioDeviceToUse != null) captureMode = StreamingCaptureMode.Audio;

            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = captureMode,
                VideoDeviceId = VideoDeviceToUse == null ? String.Empty : VideoDeviceToUse.Id,
                AudioDeviceId = AudioDeviceToUse == null ? String.Empty : AudioDeviceToUse.Id,
            };

            var captureManager = new MediaCapture();
            await captureManager.InitializeAsync(settings);

            _captureManager = captureManager;
            OnCaptureHasBeenReset();
        }
    }

    public class MediaCaptureJob
    {
        private readonly MediaCapture _captureManager;
        private readonly IStorageFile _capturedFile;

        public MediaCaptureJob(MediaCapture captureManager, IStorageFile fileToCapture)
        {
            if (captureManager == null) throw new ArgumentNullException("captureManager");
            if (fileToCapture == null) throw new ArgumentNullException("fileToCapture");
            _captureManager = captureManager;
            _capturedFile = fileToCapture;
        }

        public async void StartCaptureAsync(MediaEncodingProfile profile)
        {
            await _captureManager.StartRecordToStorageFileAsync(profile, _capturedFile);
        }

        public async Task<IStorageFile> StopCaptureAsync()
        {
            await _captureManager.StopRecordAsync();
            return _capturedFile;
        }
    }
}