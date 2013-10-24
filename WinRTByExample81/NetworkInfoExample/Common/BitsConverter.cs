namespace NetworkInfoExample.Common
{
    using System;

    using Windows.UI.Xaml.Data;

    public class BitsConverter : IValueConverter
    {
        private const long Kilobit = 1024;

        private const long Megabit = Kilobit * Kilobit;

        private const long Gigabit = Megabit * Kilobit;

        private const string Nothing = "0 bps";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Nothing;
            }

            var val = value.ToString();

            long bits;

            if (!long.TryParse(val, out bits))
            {
                return Nothing;
            }

            if (bits < Kilobit)
            {
                return string.Format("{0} bps", bits);
            }
                
            if (bits < Megabit)
            {
                return string.Format("{0} kbps", bits / Kilobit);
            }

            if (bits < Gigabit)
            {
                return string.Format("{0} mbps", bits / Megabit);
            }

            return string.Format("{0} gbps", bits / Gigabit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}