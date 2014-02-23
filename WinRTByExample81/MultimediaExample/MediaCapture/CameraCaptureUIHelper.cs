using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MultimediaExample
{
    public static class CameraCaptureUIHelper
    {
        public static async Task<IStorageFile> CaptureAsync(CameraCaptureUIMode captureMode)
        {
            var cameraUI = new CameraCaptureUI();
            var capturedMedia = await cameraUI.CaptureFileAsync(captureMode);
            if (capturedMedia == null) return null;

            // Set the default save to location based on the content MIME type
            var contentType = capturedMedia.ContentType;
            var defaultLocation = contentType.StartsWith("image")
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
            
            // Move the file return the new file
            await capturedMedia.MoveAndReplaceAsync(fileToSaveTo);
            return fileToSaveTo;
        }
    }
}