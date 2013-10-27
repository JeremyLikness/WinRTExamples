using System;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace IntegrationExample.Data
{
    public class FileInfo
    {
        public String Title { get; set; }

        public ImageSource Image { get; set; }

        public String Description { get; set; }

        public IStorageFile File { get; set; }
    }
}