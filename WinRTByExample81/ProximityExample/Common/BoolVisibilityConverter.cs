using System;

namespace ProximityExample.Common
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var visible = false;
            if (value is bool)
            {
                visible = (bool)value;
            }
            if (parameter is string && (string)parameter == "reverse")
            {
                visible = !visible;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var visible = false;
            if (value is Visibility)
            {
                visible = (Visibility)value == Visibility.Visible;
            }
            if (parameter is string && (string)parameter == "reverse")
            {
                visible = !visible;
            }            
            return visible; 
        }
    }
}
