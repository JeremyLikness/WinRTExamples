using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace MultimediaExample
{
    public class MultimediaFileDetails
    {
        #region Fields

        private readonly ObservableCollection<FileMarker> _fileMarkers = new ObservableCollection<FileMarker>(); 

        #endregion

        /// <summary>
        /// Gets or sets the playback file.
        /// </summary>
        /// <value>
        /// The playback file.
        /// </value>
        public IStorageFile PlaybackFile { get; set; }

        /// <summary>
        /// Gets the file markers associated with the current file.
        /// </summary>
        /// <value>
        /// The file markers.
        /// </value>
        public IList<FileMarker> FileMarkers { get { return _fileMarkers; } }
    }
}