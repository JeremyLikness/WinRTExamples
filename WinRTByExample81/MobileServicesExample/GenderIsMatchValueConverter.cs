using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MobileServicesExample
{
    public class GenderIsMatchValueConverter : IValueConverter
    {
        public GenderIsMatchValueConverter()
        {
            TargetGender = Gender.Male;
        }

        public Gender TargetGender { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            Gender genderValue;
            Enum.TryParse(value.ToString(), true, out genderValue);
            return genderValue == TargetGender;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            var isMatchValue = (Boolean?) value;
            return isMatchValue.GetValueOrDefault() ? TargetGender : DependencyProperty.UnsetValue;
        }
    }
}