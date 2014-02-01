namespace AccessibilityExample
{
    using System;

    using Windows.UI.Xaml.Data;

    public class AccessibleItemConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as Item;

            if (item == null)
            {
                return string.Empty;
            }

            if (parameter != null && parameter.ToString().Equals("id", StringComparison.CurrentCultureIgnoreCase))
            {
                return string.Format("ListItemId{0}", item.Id);
            }

            return item.Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}