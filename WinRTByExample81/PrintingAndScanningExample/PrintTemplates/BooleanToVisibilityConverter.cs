using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace PrintingAndScanningExample
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        private Boolean _visibleValue = true;

        public Boolean VisibleValue
        {
            get { return _visibleValue; }
            set { _visibleValue = value; }
        }

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var booleanValue = (Boolean) value;
            return booleanValue == VisibleValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            var visibilityValue = (Visibility) value;
            return VisibleValue 
                ? visibilityValue == Visibility.Visible 
                : visibilityValue != Visibility.Visible;
        }
    }
}