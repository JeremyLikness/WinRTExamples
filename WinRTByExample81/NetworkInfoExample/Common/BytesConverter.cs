namespace NetworkInfoExample.Common
{
    using System;

    using Windows.UI.Xaml.Data;

    public class BytesConverter : IValueConverter
    {
        private const long Kilobyte = 1024;

        private const long Megabyte = Kilobyte * Kilobyte;

        private const long Gigabyte = Megabyte * Kilobyte;

        private const string Nothing = "0 bytes";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Nothing;
            }

            var val = value.ToString();

            long bytes;

            if (!long.TryParse(val, out bytes))
            {
                return Nothing;
            }

            if (bytes < Kilobyte)
            {
                return string.Format("{0} bytes", bytes);
            }

            if (bytes < Megabyte)
            {
                return string.Format("{0} KB", bytes / Kilobyte);
            }

            if (bytes < Gigabyte)
            {
                return string.Format("{0} MB", bytes / Megabyte);
            }

            return string.Format("{0} GB", bytes / Gigabyte);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}