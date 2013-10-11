using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ShareTargetExample.Common
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public BooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
        }

        public Visibility TrueValue { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            return (value is Boolean && (Boolean) value)
                ? TrueValue
                : TrueValue == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            return value is Visibility && (Visibility)value == TrueValue;
        }
    }
}
