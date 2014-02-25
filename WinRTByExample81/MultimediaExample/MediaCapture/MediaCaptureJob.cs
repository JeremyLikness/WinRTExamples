using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MultimediaExample
{
    public class MediaCaptureJob
    {
        #region Constants 

        public const String WindowsMediaExtension = ".wmv";
        public const String Mp4Extension = ".mp4";

        #endregion

        #region Fields

        private readonly MediaCapture _captureManager;
        private readonly IStorageFile _fileBeingCaptured;

        #endregion

        public static async Task<MediaCaptureJob> CreateCaptureToFileJobAsync(MediaCapture captureManager)
        {
            // Save type options
            var wmvFileSaveType = new KeyValuePair<String, IList<String>>("Windows Media", new List<String> { MediaCaptureJob.WindowsMediaExtension });
            var mp4FileSaveType = new KeyValuePair<String, IList<String>>("MP4", new List<String> { MediaCaptureJob.Mp4Extension });

            // Get the file to save
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
                SuggestedFileName = "Video Capture"
            };
            savePicker.FileTypeChoices.Add(wmvFileSaveType);
            savePicker.FileTypeChoices.Add(mp4FileSaveType);
            var fileToSaveTo = await savePicker.PickSaveFileAsync();
            if (fileToSaveTo == null) return null;

            var mediaCaptureJob = new MediaCaptureJob(captureManager, fileToSaveTo);
            return mediaCaptureJob;
        }

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
        private MediaCaptureJob(MediaCapture captureManager, IStorageFile fileToCapture)
        {
            if (captureManager == null) throw new ArgumentNullException("captureManager");
            if (fileToCapture == null) throw new ArgumentNullException("fileToCapture");
            _captureManager = captureManager;
            _fileBeingCaptured = fileToCapture;
        } 

        #endregion

        public async void StartCaptureAsync(VideoEncodingQuality captureQuality)
        {
            // Build the media encoding profile from the selected file type and
            MediaEncodingProfile profile;
            switch (_fileBeingCaptured.FileType)
            {
                case WindowsMediaExtension:
                    profile = MediaEncodingProfile.CreateWmv(captureQuality);
                    break;
                case Mp4Extension:
                    profile = MediaEncodingProfile.CreateMp4(captureQuality);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file type");
            }

            await _captureManager.StartRecordToStorageFileAsync
                (profile, _fileBeingCaptured);
        }

        public async Task<IStorageFile> StopCaptureAsync()
        {
            await _captureManager.StopRecordAsync();
            return _fileBeingCaptured;
        }
    }
}