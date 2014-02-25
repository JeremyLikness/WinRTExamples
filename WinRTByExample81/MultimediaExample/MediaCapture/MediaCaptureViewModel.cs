using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Media.MediaProperties;
using MultimediaExample.Annotations;
using MultimediaExample.Common;

namespace MultimediaExample
{
    public class MediaCaptureViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly MediaCaptureHelper _mediaCaptureHelper;

        private MediaCaptureJob _currentCaptureJob;
        private Boolean _isCapturing;

        private readonly ObservableCollection<DeviceInformation> _audioCaptureDevices = new ObservableCollection<DeviceInformation>();
        private readonly ObservableCollection<DeviceInformation> _videoCaptureDevices = new ObservableCollection<DeviceInformation>();

        private Boolean _isPreviewMirrored;
        
        private RelayCommand _showCameraSettingsCommand;
        private RelayCommand _startCaptureCommand;
        private RelayCommand _stopCaptureCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCaptureViewModel"/> class.
        /// </summary>
        /// <param name="mediaCaptureHelper">The media capture helper.</param>
        /// <exception cref="System.ArgumentNullException">mediaCaptureHelper</exception>
        public MediaCaptureViewModel(MediaCaptureHelper mediaCaptureHelper)
        {
            if (mediaCaptureHelper == null) throw new ArgumentNullException("mediaCaptureHelper");
            _mediaCaptureHelper = mediaCaptureHelper;
        } 

        #endregion

        #region Capture device enumeration and selection

        public async void UpdateCaptureDevices()
        {
            var audioCaptureDevices = await _mediaCaptureHelper.GetAudioCaptureDevicesAsync();
            _audioCaptureDevices.Clear();
            foreach (var device in audioCaptureDevices)
            {
                _audioCaptureDevices.Add(device);
            }

            var videoCaptureDevices = await _mediaCaptureHelper.GetVideoCaptureDevicesAsync();
            _videoCaptureDevices.Clear();
            foreach (var device in videoCaptureDevices)
            {
                _videoCaptureDevices.Add(device);
            }
        }

        public IEnumerable<DeviceInformation> AudioCaptureDevices
        {
            get { return _audioCaptureDevices; }
        }

        public DeviceInformation SelectedAudioCaptureDevice
        {
            get { return _mediaCaptureHelper.AudioDeviceToUse; }
            set
            {
                if (Equals(value, _mediaCaptureHelper.AudioDeviceToUse)) return;
                _mediaCaptureHelper.AudioDeviceToUse = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<DeviceInformation> VideoCaptureDevices
        {
            get { return _videoCaptureDevices; }
        }

        public DeviceInformation SelectedVideoCaptureDevice
        {
            get { return _mediaCaptureHelper.VideoDeviceToUse; }
            set
            {
                if (Equals(value, _mediaCaptureHelper.VideoDeviceToUse)) return;
                _mediaCaptureHelper.VideoDeviceToUse = value;
                OnPropertyChanged();
            }
        } 

        #endregion

        #region Video quality enumeration & selection

        public IEnumerable<VideoEncodingQuality> VideoQualities
        {
            get { return _mediaCaptureHelper.VideoQualities; }
        }

        public VideoEncodingQuality SelectedVideoQuality
        {
            get { return _mediaCaptureHelper.VideoQualityToUse; }
            set
            {
                if (Equals(value, _mediaCaptureHelper.VideoQualityToUse)) return;
                _mediaCaptureHelper.VideoQualityToUse = value;
                OnPropertyChanged();
            }
        } 

        #endregion

        #region Preview Settings

        public Boolean IsPreviewMirrored
        {
            get { return _isPreviewMirrored; }
            set
            {
                if (value.Equals(_isPreviewMirrored)) return;
                _isPreviewMirrored = value;
                _mediaCaptureHelper.SetPreviewMirroring(value);
                OnPropertyChanged();
            }
        } 

        #endregion

        #region Capturing status

        public Boolean IsCapturing
        {
            get { return _isCapturing; }
            private set
            {
                if (value.Equals(_isCapturing)) return;
                _isCapturing = value;
                OnPropertyChanged();
            }
        } 

        #endregion

        #region Commands

        public ICommand ShowCameraSettingsCommand
        {
            get { return _showCameraSettingsCommand ?? (_showCameraSettingsCommand = new RelayCommand(ShowCameraSettings)); }
        }

        public ICommand StartCaptureCommand
        {
            get { return _startCaptureCommand ?? (_startCaptureCommand = new RelayCommand(StartCapture)); }
        }

        public ICommand StopCaptureCommand
        {
            get { return _stopCaptureCommand ?? (_stopCaptureCommand = new RelayCommand(StopCapture)); }
        } 

        #endregion

        #region Command Implementations

        private void ShowCameraSettings()
        {
            _mediaCaptureHelper.ShowCameraSettings();
        }

        private async void StartCapture()
        {
            // Clear out any previous capture job
            _currentCaptureJob = null;

            // Start a new capture job
            var captureJob = await _mediaCaptureHelper.StartCaptureAsync();
            if (captureJob != null)
            {
                IsCapturing = true;
                _currentCaptureJob = captureJob;
            }
        }

        private async void StopCapture()
        {
            IsCapturing = false;
            await _currentCaptureJob.StopCaptureAsync();
        } 

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}