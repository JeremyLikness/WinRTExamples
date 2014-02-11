using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Media.MediaProperties;
using Windows.Storage;
using MultimediaExample.Annotations;
using MultimediaExample.Common;

namespace MultimediaExample
{
    public class MediaCaptureViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly MediaCaptureHelper _mediaCaptureHelper;

        private String _statusText;

        private MediaCaptureJob _currentCaptureJob;
        private Boolean _isCapturing;

        private readonly ObservableCollection<DeviceInformation> _audioCaptureDevices = new ObservableCollection<DeviceInformation>();
        private readonly ObservableCollection<DeviceInformation> _videoCaptureDevices = new ObservableCollection<DeviceInformation>();
        
        private RelayCommand _showSettingsCommand;
        private RelayCommand _startCaptureCommand;
        private RelayCommand _stopCaptureCommand;

        #endregion

        public MediaCaptureViewModel(MediaCaptureHelper mediaCaptureHelper)
        {
            if (mediaCaptureHelper == null) throw new ArgumentNullException("mediaCaptureHelper");
            _mediaCaptureHelper = mediaCaptureHelper;
        }

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

        public String StatusText
        {
            get { return _statusText; }
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
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

        public ICommand ShowSettingsCommand
        {
            get { return _showSettingsCommand ?? (_showSettingsCommand = new RelayCommand(ShowSettings)); }
        }

        public ICommand StartCaptureCommand
        {
            get { return _startCaptureCommand ?? (_startCaptureCommand = new RelayCommand(StartCapture)); }
        }

        public ICommand StopCaptureCommand
        {
            get { return _stopCaptureCommand ?? (_stopCaptureCommand = new RelayCommand(StopCapture)); }
        }

        private void ShowSettings()
        {
            _mediaCaptureHelper.ShowSettings();
        }

        private async void StartCapture()
        {
            _currentCaptureJob = await _mediaCaptureHelper.StartCaptureAsync();
            if (_currentCaptureJob != null)
            {
                IsCapturing = true;
            }
        }

        private async void StopCapture()
        {
            var capturedFile = await _currentCaptureJob.StopCaptureAsync();
            IsCapturing = false;
            OnCaptureCompleted(capturedFile);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<CaptureCompletedEventArgs> CaptureCompleted;

        private void OnCaptureCompleted(IStorageFile capturedFile)
        {
            var handler = CaptureCompleted;
            if (handler != null) handler(this, new CaptureCompletedEventArgs(capturedFile));
        }
    }

    public class CaptureCompletedEventArgs : EventArgs
    {
        private readonly IStorageFile _capturedFile;

        public CaptureCompletedEventArgs(IStorageFile capturedFile)
        {
            if (capturedFile == null) throw new ArgumentNullException("capturedFile");
            _capturedFile = capturedFile;
        }

        public IStorageFile CapturedFile
        {
            get { return _capturedFile; }
        }
    }
}