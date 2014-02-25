using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
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

            Application.Current.Suspending += async (o, e) =>
            {
                if (_captureManager == null) return;

                // Stop previewing when the app is being moved from the foreground
                var deferral = e.SuspendingOperation.GetDeferral();
                await _captureManager.StopPreviewAsync();
                _captureManager = null;
                deferral.Complete();
            };

            Application.Current.Resuming += (o, e) =>
            {
                // Reset/restart on application resume
                ApplyDeviceSettings();
            };
        }

        #endregion

        #region Capture device enumeration & selection

        public async Task<IEnumerable<DeviceInformation>> GetVideoCaptureDevicesAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return devices;
        }

        public async Task<IEnumerable<DeviceInformation>> GetAudioCaptureDevicesAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            return devices;
        }

        public DeviceInformation VideoDeviceToUse
        {
            get { return _videoDeviceToUse; }
            set
            {
                _videoDeviceToUse = value;
                ApplyDeviceSettings();
            }
        }

        public DeviceInformation AudioDeviceToUse
        {
            get { return _audioDeviceToUse; }
            set
            {
                _audioDeviceToUse = value;
                ApplyDeviceSettings();
            }
        }

        private async void ApplyDeviceSettings()
        {
            // Determine the current capture mode, based on device selections
            var captureMode = StreamingCaptureMode.AudioAndVideo;
            if (VideoDeviceToUse == null && AudioDeviceToUse == null) return;
            if (VideoDeviceToUse != null && AudioDeviceToUse == null) 
                captureMode = StreamingCaptureMode.Video;
            if (VideoDeviceToUse == null && AudioDeviceToUse != null) 
                captureMode = StreamingCaptureMode.Audio;

            // Set up the initialization settings
            var settings = new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = captureMode,
                VideoDeviceId = VideoDeviceToUse == null 
                                        ? String.Empty 
                                        : VideoDeviceToUse.Id,
                AudioDeviceId = AudioDeviceToUse == null 
                                        ? String.Empty 
                                        : AudioDeviceToUse.Id,
            };

            // Create and initialize a new MediaCapture instance
            var captureManager = new MediaCapture();
            try
            {
                await captureManager.InitializeAsync(settings);
            }
            catch (UnauthorizedAccessException)
            {
                // The user has declined/blocked access to the camera/microphone
                // Prompt the user properly to enable access
                return;
            }
            _captureManager = captureManager;

            // Raise the CaptureSettingsReset event
            OnCaptureSettingsReset();
        }

        public event EventHandler CaptureSettingsReset;

        private void OnCaptureSettingsReset()
        {
            var handler = CaptureSettingsReset;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion

        #region Capture preview

        /// <summary>
        /// Starts the capture preview.
        /// </summary>
        /// <param name="captureUIElement">The capture UI element.</param>
        /// <exception cref="System.ArgumentNullException">captureUIElement</exception>
        public async void StartCapturePreview(CaptureElement captureUIElement)
        {
            if (captureUIElement == null) throw new ArgumentNullException("captureUIElement");

            // Associate the MediaCapture instance with the CaptureElement 
            // and start video preview
            var captureMode = _captureManager.MediaCaptureSettings.StreamingCaptureMode;
            if (captureMode == StreamingCaptureMode.Audio)
            {
                // No video to capture.  We're done.
                return;
            }

            captureUIElement.Source = _captureManager;
            await _captureManager.StartPreviewAsync();
        }

        public void SetPreviewMirroring(Boolean isPreviewMirrored)
        {
            _captureManager.SetPreviewMirroring(isPreviewMirrored);
        }

        #endregion

        #region Video quality enumeration & selection

        public IEnumerable<VideoEncodingQuality> VideoQualities
        {
            get { return Enum.GetValues(typeof(VideoEncodingQuality)).Cast<VideoEncodingQuality>(); }
        }

        public VideoEncodingQuality VideoQualityToUse { get; set; }

        #endregion

        #region Actual capture

        public async Task<MediaCaptureJob> StartCaptureAsync()
        {
            if (_captureManager == null) throw new InvalidOperationException("Capture Manager has not been initialized.");
            var captureJob = await MediaCaptureJob.CreateCaptureToFileJobAsync(_captureManager);
            if (captureJob != null)
            {
                captureJob.StartCaptureAsync(VideoQualityToUse);
            }
            return captureJob;
        } 

        #endregion

        #region Camera settings

        public void ShowCameraSettings()
        {
            // If the capture manager has not been initialized, nothing to show
            if (_captureManager == null) return;

            var captureMode = _captureManager.MediaCaptureSettings.StreamingCaptureMode;
            if (captureMode == StreamingCaptureMode.Audio)
            {
                // No video to capture.  We're done.
                return;
            }

            // Display capture options UI
            CameraOptionsUI.Show(_captureManager);
        }

        #endregion

    }
}