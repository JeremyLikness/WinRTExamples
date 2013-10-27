using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace IntegrationExample.Data
{
    public static partial class Extensions
    {
        public static async Task<List<FileInfo>> GetRelatedFiles(this Contact currentContact)
        {
            if (currentContact == null) throw new ArgumentNullException("currentContact");

            var results = new List<FileInfo>();

            var localFolder = ApplicationData.Current.LocalFolder;
            var selectedContactFolder =
                await localFolder.CreateFolderAsync(currentContact.Id, CreationCollisionOption.OpenIfExists);
            var rawFiles = await selectedContactFolder.GetFilesAsync();
            foreach (var rawFile in rawFiles)
            {
                var thumbnail = await rawFile.GetThumbnailAsync(ThumbnailMode.ListView);
                if (thumbnail != null)
                {
                    var image = new BitmapImage();
                    image.SetSource(thumbnail);
                    var result = new FileInfo
                                 {
                                     Title = rawFile.Name,
                                     Image = image,
                                     File = rawFile,
                                 };
                    results.Add(result);
                }
            }
            return results;
        }

        public static async Task<StorageFile> SaveRelatedFile(this Contact currentContact, String desiredFileName)
        {
            if (currentContact == null) throw new ArgumentNullException("currentContact");

            // Put the incoming file into a folder keyed by the selected contact's Id
            var localFolder = ApplicationData.Current.LocalFolder;
            var selectedContactFolder =
                await localFolder.CreateFolderAsync(currentContact.Id, CreationCollisionOption.OpenIfExists);

            var result = await selectedContactFolder.CreateFileAsync(desiredFileName, CreationCollisionOption.ReplaceExisting);
            return result;
        }
    }
}