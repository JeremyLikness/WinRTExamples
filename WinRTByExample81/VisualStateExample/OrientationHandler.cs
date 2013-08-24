namespace VisualStateExample
{
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public static class OrientationHandler
    {
        public static readonly DependencyProperty HandleOrientationProperty =
            DependencyProperty.RegisterAttached(
                "HandleOrientation",
                typeof(bool),
                typeof(OrientationHandler),
                new PropertyMetadata(false, OnHandleOrientationChanged));

        public static void SetHandleOrientation(UIElement element, bool value)
        {
            element.SetValue(HandleOrientationProperty, value);
        }

        public static bool GetHandleOrientation(UIElement element)
        {
            return (bool)element.GetValue(HandleOrientationProperty);
        }

        public static readonly DependencyProperty LastOrientationProperty =
            DependencyProperty.RegisterAttached(
                "LastOrientation",
                typeof(string),
                typeof(OrientationHandler),
                new PropertyMetadata(string.Empty));

        public static void SetLastOrientation(UIElement element, string value)
        {
            element.SetValue(LastOrientationProperty, value);
        }

        public static string GetLastOrientation(UIElement element)
        {
            return (string)element.GetValue(LastOrientationProperty);
        }
        private static void OnHandleOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Control;
            
            if (control == null)
            {
                return;
            }
            
            control.Loaded += (sender, args) => SetLayout(control);
            control.LayoutUpdated += (sender, args) => SetLayout(control);
            control.SizeChanged += (sender, args) => SetLayout(control);
        }

        private static void SetLayout(Control control)
        {
            var orientation = ApplicationView.GetForCurrentView().Orientation;
            string newMode;

            if (orientation == ApplicationViewOrientation.Landscape)
            {
                newMode = ApplicationView.GetForCurrentView().IsFullScreen ? "FullScreenLandscape" : "Filled";
            }
            else
            {
                newMode = Window.Current.Bounds.Width <= 500 ? "Snapped" : "FullScreenPortrait";
            }

            if (newMode == GetLastOrientation(control))
            {
                return;
            }

            VisualStateManager.GoToState(control, newMode, true);
            SetLastOrientation(control, newMode);
        }
    }
}
