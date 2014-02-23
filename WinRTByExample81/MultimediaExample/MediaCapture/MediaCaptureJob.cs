using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace MultimediaExample
{
    public class MediaCaptureJob
    {
        #region Fields

        private readonly MediaCapture _captureManager;
        private readonly IStorageFile _capturedFile;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCaptureJob"/> class.
        /// </summary>
        /// <param name="captureManager">The capture manager.</param>
        /// <param name="fileToCapture">The file to capture.</param>
        /// <exception cref="System.ArgumentNullException">
        /// captureManager
        /// or
        /// fileToCapture
        /// </exception>
        public MediaCaptureJob(MediaCapture captureManager, IStorageFile fileToCapture)
        {
            if (captureManager == null) throw new ArgumentNullException("captureManager");
            if (fileToCapture == null) throw new ArgumentNullException("fileToCapture");
            _captureManager = captureManager;
            _capturedFile = fileToCapture;
        } 

        #endregion

        public async void StartCaptureAsync(MediaEncodingProfile profile)
        {
            await _captureManager.StartRecordToStorageFileAsync(profile, _capturedFile);
        }

        public async Task<IStorageFile> StopCaptureAsync()
        {
            await _captureManager.StopRecordAsync();
            return _capturedFile;
        }
    }
}