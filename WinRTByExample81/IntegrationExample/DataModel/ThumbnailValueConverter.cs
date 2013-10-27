using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace IntegrationExample.Data
{
    public class ThumbnailValueConverter : IValueConverter
    {
        public ThumbnailValueConverter()
        {
            // Set the default/fallback image to use when one is not set in the markup
            ProfileImagePath = "ms-appx:///Assets/Profile.png";
        }

        public String ProfileImagePath { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var stream = value as IRandomAccessStream;
            if (stream != null)
            {
                var image = new BitmapImage();
                image.SetSource(stream);
                return image;
            }
            else
            {
                // The bound image was empty, use the default/fallback/empty value
                var image = new BitmapImage { UriSource = new Uri(ProfileImagePath) };
                return image;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}