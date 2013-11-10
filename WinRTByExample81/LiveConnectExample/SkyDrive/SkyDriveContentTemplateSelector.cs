using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveConnectExample
{
    public class SkyDriveContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderOrAlbumTemplate { get; set; }

        public DataTemplate AudioVideoTemplate { get; set; }

        public DataTemplate PhotoTemplate { get; set; }

        public DataTemplate EmbeddableFileTemplate { get; set; }

        /// <summary>
        /// When implemented by a derived class, returns a specific DataTemplate for a given item or container.
        /// </summary>
        /// <param name="item">The item to return a template for.</param>
        /// <param name="container">The parent container for the templated item.</param>
        /// <returns>
        /// The template to use for the given item and/or container.
        /// </returns>
        protected override DataTemplate SelectTemplateCore(Object item, DependencyObject container)
        {
            var skyDriveItem = (dynamic) item;
            String itemType = item == null ? String.Empty : skyDriveItem.type.ToString();
            switch (itemType)
            {
                case "folder":
                case "album":
                    return FolderOrAlbumTemplate;
                case "photo":
                    return PhotoTemplate;
                case "video":
                case "audio":
                    return AudioVideoTemplate;
                case "file":
                case "notebook":
                    return EmbeddableFileTemplate;
                default:
                    return base.SelectTemplateCore(item, container);
            }
        }
    }
}