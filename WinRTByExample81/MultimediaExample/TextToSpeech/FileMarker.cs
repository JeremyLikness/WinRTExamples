using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MultimediaExample.Annotations;

namespace MultimediaExample
{
    public class FileMarker : INotifyPropertyChanged
    {
        #region Fields

        private String _name;
        private TimeSpan _time;
        private String _textToSpeechContent;
        private Boolean _isSsml;
        private String _selectedVoiceId = TextToSpeechHelper.DefaultVoice.Id;

        #endregion

        #region Properties

        public String Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                if (value.Equals(_time)) return;
                _time = value;
                OnPropertyChanged();
            }
        }

        public String TextToSpeechContent
        {
            get { return _textToSpeechContent; }
            set
            {
                if (value == _textToSpeechContent) return;
                _textToSpeechContent = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsSsml
        {
            get { return _isSsml; }
            set
            {
                if (value.Equals(_isSsml)) return;
                _isSsml = value;
                OnPropertyChanged();
            }
        }

        public String SelectedVoiceId
        {
            get { return _selectedVoiceId; }
            set
            {
                if (Equals(value, _selectedVoiceId)) return;
                _selectedVoiceId = value;
                OnPropertyChanged();
            }
        } 

        #endregion

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
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }
}