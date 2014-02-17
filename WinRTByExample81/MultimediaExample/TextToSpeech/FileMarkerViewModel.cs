using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Media.SpeechSynthesis;
using MultimediaExample.Annotations;
using MultimediaExample.Common;

namespace MultimediaExample
{
    public class FileMarkerViewModel : INotifyPropertyChanged
    {
        #region Fields

        private RelayCommand _speakCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMarkerViewModel"/> class.
        /// </summary>
        /// <param name="multimediaViewModel">The multimedia view model.</param>
        /// <param name="fileMarker">The file marker.</param>
        public FileMarkerViewModel(MultimediaViewModel multimediaViewModel, FileMarker fileMarker)
        {
            MultimediaViewModel = multimediaViewModel;
            FileMarker = fileMarker;
        } 

        #endregion

        public MultimediaViewModel MultimediaViewModel { get; private set; }

        public FileMarker FileMarker { get; private set; }

        public IEnumerable<VoiceInformation> Voices
        {
            get { return TextToSpeechHelper.Voices; }
        }

        public ICommand SpeakCommand
        {
            get { return _speakCommand ?? (_speakCommand = new RelayCommand(Speak)); }
        }

        private void Speak()
        {
            TextToSpeechHelper.PlayContentAsync(FileMarker.TextToSpeechContent, FileMarker.IsSsml, FileMarker.SelectedVoiceId);
        }

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }
}