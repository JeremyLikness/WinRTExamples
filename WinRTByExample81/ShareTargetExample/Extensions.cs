using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ShareTargetExample
{
    // Attached property for Binding String Text directly to a WebView control.  Based on Tim Heuer's Callisto WebViewExtension at https://github.com/timheuer/callisto
    public partial class Extensions
    {
        /// <summary>
        /// Gets the source HTML text.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static String GetSourceHtmlText(WebView obj)
        {
            return (String)obj.GetValue(SourceHtmlTextProperty);
        }

        /// <summary>
        /// Sets the source HTML text.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetSourceHtmlText(WebView obj, String value)
        {
            obj.SetValue(SourceHtmlTextProperty, value);
        }

        /// <summary>
        /// The source HTML text property
        /// </summary>
        public static readonly DependencyProperty SourceHtmlTextProperty =
            DependencyProperty.RegisterAttached("SourceHtmlText", typeof(String), typeof(Extensions), new PropertyMetadata(default(String), OnSourceHtmlTextChanged));

        /// <summary>
        /// Called when the source HTML text value is changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="Windows.UI.Xaml.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSourceHtmlTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var webView = (WebView)dependencyObject;
            var sourceHtmlText = args.NewValue as String;
            if (String.IsNullOrWhiteSpace(sourceHtmlText)) sourceHtmlText = "<html></html>";
            webView.NavigateToString(sourceHtmlText);
        }
    }
}