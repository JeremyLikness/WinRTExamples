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
using MultimediaExample.Annotations;
using MultimediaExample.Common;

namespace MultimediaExample
{
    public class PlaybackViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly IPlaybackWindow _playbackWindowProxy;
        private readonly Action<Type> _navigationCallback;

        private readonly ObservableCollection<FileData> _playbackFiles = new ObservableCollection<FileData>();
        private FileData _currentPlaybackFile;
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
        /// Initializes a new instance of the <see cref="PlaybackViewModel" /> class.
        /// </summary>
        /// <param name="navigationCallback">The navigation callback.</param>
        /// <param name="playbackWindowProxy">The playback window proxy.</param>
        /// <exception cref="System.ArgumentNullException">canAddFileCallback</exception>
        public PlaybackViewModel(IPlaybackWindow playbackWindowProxy, Action<Type> navigationCallback)
        {
            if (DesignMode.DesignModeEnabled) return;

            if (playbackWindowProxy == null) throw new ArgumentNullException("playbackWindowProxy");
            if (navigationCallback == null) throw new ArgumentNullException("navigationCallback");

            _playbackWindowProxy = playbackWindowProxy;
            _navigationCallback = navigationCallback;
        } 

        #endregion

        #region Methods

        public TimeSpan GetCurrentPlaybackPosition()
        {
            return _playbackWindowProxy.CurrentPosition;
        } 

        #endregion

        #region Properties

        public IList<FileData> PlaybackFiles
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
                _playbackWindowProxy.SetSlowMotion(IsSlowMotionPlayback);
            }
        }

        public FileData CurrentPlaybackFile
        {
            get { return _currentPlaybackFile; }
            set
            {
                if (Equals(value, _currentPlaybackFile)) return;
                _currentPlaybackFile = value;
                OnPropertyChanged();

                UpdateCommands();

                _playbackWindowProxy.SetSource(CurrentPlaybackFile == null ? null : CurrentPlaybackFile.PlaybackFile);

                // Reset the markers
                _playbackWindowProxy.ClearMarkers();
                if (CurrentPlaybackFile != null)
                {
                    foreach (var marker in CurrentPlaybackFile.FileMarkers)
                    {
                        _playbackWindowProxy.AddMarker(marker.Name, marker.Time);
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
            _navigationCallback.Invoke(typeof(MediaCapturePage));
        }
        
        private void RemoveSelectedVideo()
        {
            _playbackWindowProxy.Stop();
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
            _playbackWindowProxy.AddMarker(fileMarker.Name, fileMarker.Time);
        }

        private Boolean CanAddMarker(Object parameter)
        {
            return CurrentPlaybackFile != null;
        }

        private void RemoveSelectedMarker()
        {
            if (CurrentFileMarker == null) return;
            _playbackWindowProxy.RemoveMarker(CurrentFileMarker.Time);
        }

        private Boolean CanRemoveSelectedMarker()
        {
            return CurrentFileMarker != null;
        }

        private void Play()
        {
            _playbackWindowProxy.Play();
        }

        private Boolean CanPlay()
        {
            return CurrentPlaybackFile != null;
        }

        private void Pause()
        {
            _playbackWindowProxy.Pause();
        }

        private Boolean CanPause()
        {
            return CurrentPlaybackFile != null
                && _playbackWindowProxy.CanPause();
        }

        private void Stop()
        {
            _playbackWindowProxy.Stop();
        }

        private Boolean CanStop()
        {
            return CurrentPlaybackFile != null;
        }

        private void Offset(TimeSpan timeToOffset)
        {
            _playbackWindowProxy.Offset(timeToOffset);
        }

        private Boolean CanOffset()
        {
            return CurrentPlaybackFile != null
                   && _playbackWindowProxy.CanSeek();
        }

        private void Seek(TimeSpan timeToSeek)
        {
            _playbackWindowProxy.Seek(timeToSeek);
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
                    PlaybackFiles.Add(new FileData {PlaybackFile = fileToAdd});
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
            var result = _playbackWindowProxy.IsFileTypeSupported(fileToAdd.FileType);
            return result;
        } 

        #endregion
    }

    public class FileData
    {
        private readonly ObservableCollection<FileMarker> _fileMarkers = new ObservableCollection<FileMarker>();

        public IStorageFile PlaybackFile { get; set; }

        public IList<FileMarker> FileMarkers { get { return _fileMarkers; } }
    }
}