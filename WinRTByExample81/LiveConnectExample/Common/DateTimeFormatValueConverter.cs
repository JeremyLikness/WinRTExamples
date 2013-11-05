using System;
using Windows.UI.Xaml.Data;

namespace LiveConnectExample.Common
{
    public class DateTimeFormatValueConverter : IValueConverter
    {
        public DateTimeFormatValueConverter()
        {
            FormatString = "g";
        }
        
        public String FormatString { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var valueText = (value ?? String.Empty).ToString();
            DateTimeOffset dateTimeValue;
            if (DateTimeOffset.TryParse(valueText, out dateTimeValue))
            {
                return dateTimeValue.ToString(FormatString);
            }
            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}