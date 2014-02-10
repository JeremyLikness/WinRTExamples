using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MultimediaExample
{
    public class CaptureHelper
    {
        public async Task<IEnumerable<IStorageFile>> SelectVideoFilesAsync()
        {
            // Get the file to save the content to
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
                ViewMode = PickerViewMode.Thumbnail,
            };
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            var selectedFiles = await openPicker.PickMultipleFilesAsync();
            return selectedFiles;
        }

        
        public async Task<IStorageFile> CameraUICaptureAsync(CameraCaptureUIMode captureMode)
        {
            var cameraUI = new CameraCaptureUI();
            var capturedMedia = await cameraUI.CaptureFileAsync(captureMode);

            if (capturedMedia == null) return null;

            // Set the default save to location based on the content MIME type
            var contentType = capturedMedia.ContentType;
            PickerLocationId defaultLocation =
                contentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)
                    ? PickerLocationId.PicturesLibrary
                    : PickerLocationId.VideosLibrary;

            // Save type options
            var fileSaveType = new KeyValuePair<String, IList<String>>(
                capturedMedia.DisplayType,
                new List<String> {capturedMedia.FileType});

            // Get the file to save the content to
            var savePicker = new FileSavePicker
                             {
                                 SuggestedStartLocation = defaultLocation,
                                 SuggestedFileName = capturedMedia.Name
                             };
            savePicker.FileTypeChoices.Add(fileSaveType);
            var fileToSaveTo = await savePicker.PickSaveFileAsync();
            if (fileToSaveTo == null) return null;
            
            // Perform the Save and return the file
            await capturedMedia.CopyAndReplaceAsync(fileToSaveTo);
            return fileToSaveTo;
        }

        public async Task<IStorageFile> MediaCaptureCaptureAsync()
        {
            // Save type options
            var fileSaveType = new KeyValuePair<String, IList<String>>(
                "Windows Media",
                new List<String> { ".wmv" });

            // Get the file to save
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.VideosLibrary,
                SuggestedFileName = "Video Capture"
            };
            savePicker.FileTypeChoices.Add(fileSaveType);
            var fileToSaveTo = await savePicker.PickSaveFileAsync();
            if (fileToSaveTo == null) return null;

            var captureManager = new MediaCapture();
            //captureManager.StartPreviewAsync()
            //captureManager.StartRecordToStreamAsync()
            // TODO - add setting
            var quality = VideoEncodingQuality.HD1080p;
            var profile = MediaEncodingProfile.CreateWmv(quality);
            await captureManager.StartRecordToStorageFileAsync(profile, fileToSaveTo);

            // Wait 5 seconds
            // TODO - Rip this out, replace with true "stop"
            await Task.Delay(TimeSpan.FromSeconds(5));

            await captureManager.StopRecordAsync();
            //captureManager.StopPreviewAsync();

            return fileToSaveTo;
            //TODO - wire in preview (pre-capture and in-capture)
        }
    }
}