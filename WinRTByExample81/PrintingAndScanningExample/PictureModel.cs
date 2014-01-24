using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
using PrintingAndScanningExample.Annotations;

namespace PrintingAndScanningExample
{
    public class PictureModel : INotifyPropertyChanged
    {
        #region Fields

        private String _caption;
        private BitmapImage _picture;
        private BitmapImage _thumbnail; 

        #endregion

        public String Caption
        {
            get { return _caption; }
            set
            {
                if (value == _caption) return;
                _caption = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Picture
        {
            get { return _picture; }
            set
            {
                if (Equals(value, _picture)) return;
                _picture = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (Equals(value, _thumbnail)) return;
                _thumbnail = value;
                OnPropertyChanged();
            }
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
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }
}