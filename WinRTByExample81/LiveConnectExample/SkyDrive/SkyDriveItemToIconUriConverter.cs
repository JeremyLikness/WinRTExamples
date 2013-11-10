using System;
using Windows.UI.Xaml.Data;

namespace LiveConnectExample
{
    public class SkyDriveItemToIconUriConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var skydriveItem = (dynamic) value;
            String itemType = skydriveItem.type.ToString();
            return GetItemTypeIcon(itemType);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }

        public static Uri GetItemTypeIcon(String itemType)
        {
            switch (itemType.ToLowerInvariant())
            {
                case "audio":
                    return new Uri("ms-appx:///Assets/Mobile-Phone-Music.png");
                case "video":
                    return new Uri("ms-appx:///Assets/Video-Camera.png");
                case "photo":
                    return new Uri("ms-appx:///Assets/Camera.png");
                case "notebook":
                    return new Uri("ms-appx:///Assets/OneNote.png");
                case "file":
                    return new Uri("ms-appx:///Assets/Files.png");
                case "album":
                    return new Uri("ms-appx:///Assets/Images.png");
                case "folder":
                    return new Uri("ms-appx:///Assets/Folder-Open.png");
                default:
                    return new Uri("ms-appx:///Assets/Folder-Open.png");
            }
        }
    }
}