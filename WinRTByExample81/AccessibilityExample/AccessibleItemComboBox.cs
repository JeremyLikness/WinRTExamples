namespace AccessibilityExample
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    public class AccessibleItemComboBox : ComboBox 
    {
        private readonly AccessibleItemConverter converter = new AccessibleItemConverter();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var source = element as FrameworkElement;

            if (source == null)
            {
                return;
            }

            source.SetBinding(AutomationProperties.AutomationIdProperty, new Binding
            {
                Converter = converter,
                ConverterParameter = "id"
            });

            source.SetBinding(AutomationProperties.NameProperty, new Binding
            {
                Converter = converter,
                ConverterParameter = "name"
            });
        }
    }
}
