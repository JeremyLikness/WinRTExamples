using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
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

        private readonly Func<String, Boolean> _canAddFileCallback;

        private readonly ObservableCollection<IStorageFile> _playbackFiles = new ObservableCollection<IStorageFile>();
        private IStorageFile _currentPlaybackFile;
        private RelayCommand _showCameraCaptureCommand;
        private RelayCommand _chooseFilesCommand;  

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackViewModel"/> class.
        /// </summary>
        /// <param name="canAddFileCallback">The can add file callback.</param>
        /// <exception cref="System.ArgumentNullException">canAddFileCallback</exception>
        public PlaybackViewModel([NotNull] Func<String, Boolean> canAddFileCallback)
        {
            if (canAddFileCallback == null) throw new ArgumentNullException("canAddFileCallback");
            _canAddFileCallback = canAddFileCallback;
        } 

        #endregion

        #region Properties

        public IList<IStorageFile> PlaybackFiles
        {
            get { return _playbackFiles; }
        }

        public IStorageFile CurrentPlaybackFile
        {
            get { return _currentPlaybackFile; }
            set
            {
                if (Equals(value, _currentPlaybackFile)) return;
                _currentPlaybackFile = value;
                OnPropertyChanged();
                OnCurrentPlaybackFileChanged();
            }
        } 

        #endregion

        #region Events and Invocators

        /// <summary>
        /// Occurs when the current playback file is changed.
        /// </summary>
        public event EventHandler CurrentPlaybackFileChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [current playback file changed].
        /// </summary>
        protected virtual void OnCurrentPlaybackFileChanged()
        {
            EventHandler handler = CurrentPlaybackFileChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

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

        public ICommand ChooseFilesCommand
        {
            get { return _chooseFilesCommand ?? (_chooseFilesCommand = new RelayCommand(ChooseFiles)); }
        }

        public ICommand ShowCameraCaptureCommand
        {
            get { return _showCameraCaptureCommand ?? (_showCameraCaptureCommand = new RelayCommand(ShowCameraCapture)); }
        }

        #endregion

        #region Command Implementations

        private async void ChooseFiles()
        {
            var captureHelper = new CaptureHelper();
            var selectedFiles = await captureHelper.SelectVideoFilesAsync();
            AddFiles(selectedFiles);
        }

        private async void ShowCameraCapture()
        {
            var captureHelper = new CaptureHelper();
            var capturedFile = await captureHelper.CameraUICaptureAsync(CameraCaptureUIMode.PhotoOrVideo);
            if (capturedFile != null)
            {
                AddFiles(new[] {capturedFile});
            }
        }

        #endregion

        private async void AddFiles(IEnumerable<IStorageFile> filesToAdd)
        {
            var badFiles = new List<String>();
            foreach (var fileToAdd in filesToAdd)
            {
                if (CanAddFile(fileToAdd))
                {
                    PlaybackFiles.Add(fileToAdd);
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
            var result = _canAddFileCallback(fileToAdd.FileType);
            return result;
        }
    }
}