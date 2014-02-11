using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MultimediaExample
{
    public static class FilePickerHelper
    {
        public static async Task<IEnumerable<IStorageFile>> SelectVideoFilesAsync()
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
    }
}