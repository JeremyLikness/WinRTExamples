namespace GlobalizationExample
{
    using System;

    using Windows.Globalization.DateTimeFormatting;
    using Windows.System.UserProfile;
    using Windows.UI.Xaml.Data;

    public class DateValueConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultLanguage = GlobalizationPreferences.Languages[0];
            var format = parameter as string ?? "longdate";
            var languages = string.IsNullOrEmpty(language) ? new[] { defaultLanguage } : new[] { language };           
            var formatter = new DateTimeFormatter(format, languages);
            var date = DateTime.Parse((string)value);
            return formatter.Format(date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}