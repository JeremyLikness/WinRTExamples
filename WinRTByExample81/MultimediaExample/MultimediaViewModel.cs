using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.ApplicationModel;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using MultimediaExample.Annotations;
using MultimediaExample.Common;

namespace MultimediaExample
{
    public class MultimediaViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly IMediaElementWrapper _mediaElementWrapper;
        private readonly INavigate _navigationHost;

        private readonly ObservableCollection<MultimediaFileDetails> _playbackFiles = new ObservableCollection<MultimediaFileDetails>();
        private MultimediaFileDetails _currentPlaybackFile;
        private FileMarker _currentFileMarker;
        private Boolean _isSlowMotionPlayback;

        private RelayCommand _showCameraCaptureCommand;
        private RelayCommand _showMediaCaptureCommand;
        private RelayCommand _chooseFilesCommand;

        private RelayCommand _removeSelectedVideoCommand;
        private RelayCommandEx _addMarkerCommand;
        private RelayCommand _removeSelectedMarkerCommand;

        private RelayCommand _playCommand;
        private RelayCommand _pauseCommand;
        private RelayCommand _stopCommand;
        private RelayCommand _plus5Command;
        private RelayCommand _minus5Command;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaViewModel" /> class.
        /// </summary>
        /// <param name="mediaElementwrapper">The media element wrapper.</param>
        /// <param name="navigationHost">The navigation host.</param>
        /// <exception cref="System.ArgumentNullException">canAddFileCallback</exception>
        public MultimediaViewModel(IMediaElementWrapper mediaElementwrapper, INavigate navigationHost)
        {
            if (DesignMode.DesignModeEnabled) return;

            if (mediaElementwrapper == null) throw new ArgumentNullException("mediaElementwrapper");
            if (navigationHost == null) throw new ArgumentNullException("navigationHost");
            _mediaElementWrapper = mediaElementwrapper;
            _navigationHost = navigationHost;
        } 

        #endregion

        #region Methods

        public TimeSpan GetCurrentPlaybackPosition()
        {
            return _mediaElementWrapper.CurrentPosition;
        } 

        #endregion

        #region Properties

        public IList<MultimediaFileDetails> PlaybackFiles
        {
            get { return _playbackFiles; }
        }

        public Boolean IsSlowMotionPlayback
        {
            get { return _isSlowMotionPlayback; }
            set
            {
                if (value.Equals(_isSlowMotionPlayback)) return;
                _isSlowMotionPlayback = value;
                OnPropertyChanged();
                _mediaElementWrapper.SetSlowMotion(IsSlowMotionPlayback);
            }
        }

        public MultimediaFileDetails CurrentPlaybackFile
        {
            get { return _currentPlaybackFile; }
            set
            {
                if (Equals(value, _currentPlaybackFile)) return;
                _currentPlaybackFile = value;
                OnPropertyChanged();

                UpdateCommands();

                _mediaElementWrapper.SetSource(CurrentPlaybackFile == null ? null : CurrentPlaybackFile.PlaybackFile);

                // Reset the markers
                _mediaElementWrapper.ClearAllMarkers();
                if (CurrentPlaybackFile != null)
                {
                    foreach (var marker in CurrentPlaybackFile.FileMarkers)
                    {
                        _mediaElementWrapper.AddMarker(marker.Name, marker.Time);
                    }
                }
            }
        }

        public FileMarker CurrentFileMarker
        {
            get { return _currentFileMarker; }
            set
            {
                if (Equals(value, _currentFileMarker)) return;
                _currentFileMarker = value;

                // Seek to the marker position
                if (_currentFileMarker != null)
                {
                    Seek(_currentFileMarker.Time);
                }

                UpdateCommands();

                OnPropertyChanged();
            }
        }

        #endregion

        #region Events and Invocators

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Commands

        public RelayCommand ChooseFilesCommand
        {
            get { return _chooseFilesCommand ?? (_chooseFilesCommand = new RelayCommand(ChooseFiles)); }
        }

        public RelayCommand ShowCameraCaptureCommand
        {
            get { return _showCameraCaptureCommand ?? (_showCameraCaptureCommand = new RelayCommand(ShowCameraCapture)); }
        }

        public RelayCommand ShowMediaCaptureCommand
        {
            get { return _showMediaCaptureCommand ?? (_showMediaCaptureCommand = new RelayCommand(ShowMediaCapture)); }
        }

        public RelayCommand RemoveSelectedVideoCommand
        {
            get { return _removeSelectedVideoCommand ?? (_removeSelectedVideoCommand = new RelayCommand(RemoveSelectedVideo, CanRemoveSelectedVideo)); }
        }

        public RelayCommandEx AddMarkerCommand
        {
            get { return _addMarkerCommand ?? (_addMarkerCommand = new RelayCommandEx(AddMarker, CanAddMarker)); }
        }

        public RelayCommand RemoveSelectedMarkerCommand
        {
            get {  return _removeSelectedMarkerCommand ?? (_removeSelectedMarkerCommand = new RelayCommand(RemoveSelectedMarker, CanRemoveSelectedMarker)); }
        }

        public RelayCommand PlayCommand
        {
            get { return _playCommand ?? (_playCommand = new RelayCommand(Play, CanPlay)); }
        }

        public RelayCommand PauseCommand
        {
            get { return _pauseCommand ?? (_pauseCommand = new RelayCommand(Pause, CanPause)); }
        }

        public RelayCommand StopCommand
        {
            get { return _stopCommand ?? (_stopCommand = new RelayCommand(Stop, CanStop)); }
        }

        public RelayCommand Plus5Command
        {
            get
            {
                return _plus5Command ?? 
                       (_plus5Command = new RelayCommand(() => Offset(TimeSpan.FromSeconds(5)), CanOffset));
            }
        }

        public RelayCommand Minus5Command
        {
            get
            {
                return _minus5Command ??
                       (_minus5Command = new RelayCommand(() => Offset(TimeSpan.FromSeconds(-5)), CanOffset));
            }
        }

        #endregion

        #region Command Implementations

        private void UpdateCommands()
        {
            RemoveSelectedVideoCommand.RaiseCanExecuteChanged();
            AddMarkerCommand.RaiseCanExecuteChanged();
            RemoveSelectedMarkerCommand.RaiseCanExecuteChanged();

            PlayCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            Plus5Command.RaiseCanExecuteChanged();
            Minus5Command.RaiseCanExecuteChanged();
        }

        private async void ChooseFiles()
        {
            var selectedFiles = await FilePickerHelper.SelectVideoFilesAsync();
            AddFiles(selectedFiles);
        }

        private async void ShowCameraCapture()
        {
            var capturedFile = await CaptureUIHelper.CameraUICaptureAsync(CameraCaptureUIMode.PhotoOrVideo);
            if (capturedFile != null)
            {
                AddFiles(new[] {capturedFile});
            }
        }

        private void ShowMediaCapture()
        {
            _navigationHost.Navigate(typeof(MediaCapturePage));
        }
        
        private void RemoveSelectedVideo()
        {
            _mediaElementWrapper.Stop();
            PlaybackFiles.Remove(CurrentPlaybackFile);
        }

        private Boolean CanRemoveSelectedVideo()
        {
            return CurrentPlaybackFile != null;
        }

        private void AddMarker(Object parameter)
        {
            // Drop a marker entry at the current file playback position
            var fileMarker = parameter as FileMarker;
            if (fileMarker == null) return;
            CurrentPlaybackFile.FileMarkers.Add(fileMarker);
            _mediaElementWrapper.AddMarker(fileMarker.Name, fileMarker.Time);
        }

        private Boolean CanAddMarker(Object parameter)
        {
            return CurrentPlaybackFile != null;
        }

        private void RemoveSelectedMarker()
        {
            if (CurrentFileMarker == null) return;
            _mediaElementWrapper.RemoveMarker(CurrentFileMarker.Time);
        }

        private Boolean CanRemoveSelectedMarker()
        {
            return CurrentFileMarker != null;
        }

        private void Play()
        {
            _mediaElementWrapper.Play();
        }

        private Boolean CanPlay()
        {
            return CurrentPlaybackFile != null;
        }

        private void Pause()
        {
            _mediaElementWrapper.Pause();
        }

        private Boolean CanPause()
        {
            return CurrentPlaybackFile != null
                && _mediaElementWrapper.CanPause();
        }

        private void Stop()
        {
            _mediaElementWrapper.Stop();
        }

        private Boolean CanStop()
        {
            return CurrentPlaybackFile != null;
        }

        private void Offset(TimeSpan timeToOffset)
        {
            _mediaElementWrapper.Offset(timeToOffset);
        }

        private Boolean CanOffset()
        {
            return CurrentPlaybackFile != null
                   && _mediaElementWrapper.CanSeek();
        }

        private void Seek(TimeSpan timeToSeek)
        {
            _mediaElementWrapper.Seek(timeToSeek);
        }

        #endregion

        #region Adding Files

        private async void AddFiles(IEnumerable<IStorageFile> filesToAdd)
        {
            var badFiles = new List<String>();
            foreach (var fileToAdd in filesToAdd)
            {
                if (CanAddFile(fileToAdd))
                {
                    PlaybackFiles.Add(new MultimediaFileDetails {PlaybackFile = fileToAdd});
                }
                else
                {
                    badFiles.Add(fileToAdd.Name);
                }
            }

            if (badFiles.Any())
            {
                var fileList = new StringBuilder(badFiles.First());
                foreach (var badFile in badFiles.Skip(1))
                {
                    fileList.AppendFormat(", {0}", badFile);
                }
                var badFileMessage = String.Format("Unable to add unsupported file(s): {0}", fileList);
                var messageDialog = new MessageDialog(badFileMessage, "Bad Files");
                await messageDialog.ShowAsync();
            }
        }

        private Boolean CanAddFile(IStorageFile fileToAdd)
        {
            var result = _mediaElementWrapper.IsFileTypeSupported(fileToAdd.FileType);
            return result;
        } 

        #endregion
    }
}