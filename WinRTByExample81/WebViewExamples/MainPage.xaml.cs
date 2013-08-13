// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebViewExamples
{
    using System;
    using System.Net.Http;

    using Windows.ApplicationModel.DataTransfer;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The html fragment.
        /// </summary>
        private const string HtmlFragment =
            "<html><head><title>Html Fragment</title></head><body>" +
            "<h1>This is an Example</h1>" +
            "<h2>Of a String Literal</h2>" +
            "<h3>Fragment Parsed by the WebView Control</h3>" +
            "<div>Isn't it cool?</div>" +
            "<div style=\"background: black;\"><a href=\"ms-appx-web:///Assets/Logo.png\"><img src=\"ms-appx-web:///Assets/Logo.png\"></img></a></div><br/>" +
            "<div><a href=\"http://csharperimage.jeremylikness.com/\">Get Me Out of Here!</a></div>" +
            "</body></html>";

        /// <summary>
        /// The URL of Jeremy's blog
        /// </summary>
        private const string JeremyBlog = "http://csharperimage.jeremylikness.com/";

        /// <summary>
        /// The mobile user agent.
        /// </summary>
        private const string MobileUserAgent = "Mozilla/5.0 (iPhone; U; CPU like Mac OS X; en) AppleWebKit/420+ (KHTML, like Gecko) Version/3.0 Mobile/1A543a Safari/419.3";

        /// <summary>
        /// The URL to Jeremy's blog post about the Lenovo Yoga 
        /// </summary>
        private const string JeremyYogaPost = "http://csharperimage.jeremylikness.com/2013/02/review-of-lenovo-ideapad-yoga-13-for.html";

        /// <summary>
        /// Demonstrate WebGl technology
        /// </summary>
        private const string WebGl = "http://todo/";

        /// <summary>
        /// Message from blog
        /// </summary>
        private string message = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// The main page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            this.WebViewControl.NavigationCompleted +=  this.WebViewControlNavigationCompleted;
            this.WebViewControl.ScriptNotify += this.WebViewControlScriptNotify;
        }

        /// <summary>
        /// The browser load completed message.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void WebViewControlNavigationCompleted(object sender, WebViewNavigationCompletedEventArgs e)
        {
            WebBrush.Redraw();

            var url = e.Uri != null ? e.Uri.ToString() : "Text Content";
            var popup = new MessageDialog(url, "Content Loaded.");
            await popup.ShowAsync();

            if (string.IsNullOrEmpty(this.message))
            {
                return;
            }

            popup = new MessageDialog(
                this.message,
                "The Blog Has Spoken!");
            this.message = string.Empty;
            await popup.ShowAsync();
        }

        /// <summary>
        /// The browser/s script notify.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebViewControlScriptNotify(
            object sender,
            NotifyEventArgs e)
        {
            this.message = e.Value;
        }

        /// <summary>
        /// The basic page view.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonBase_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            this.WebViewControl.Navigate(new Uri(JeremyBlog));
        }

        /// <summary>
        /// The view page with mobile agent method.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void ViewMobile_OnClick(object sender, RoutedEventArgs e)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("user-agent", MobileUserAgent);
            var response = await client.GetAsync(new Uri(JeremyYogaPost));
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var fragment = HtmlFormatHelper.GetStaticFragment(HtmlFormatHelper.CreateHtmlFormat(html));
            this.WebViewControl.NavigateToString(fragment);
        }

        /// <summary>
        /// The view SVG method.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ViewSvg_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            this.WebViewControl.Navigate(new
                Uri("ms-appx-web:///Data/Ellipse.html"));
        }

        /// <summary>
        /// The view string literal method.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ViewString_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            this.WebViewControl.NavigateToString(HtmlFragment);
        }

        /// <summary>
        /// The call JavaScript method
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void CallJavaScript_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            MessageDialog popup = null;

            var parameters = new[]
                                    {
                                        "p/biography.html"
                                    };
            try
            {
                this.WebViewControl.InvokeScript(
                    "superSecretBiographyFunction",
                    parameters);
            }
            catch (Exception ex)
            {
                popup = new MessageDialog(
                    ex.Message,
                    "Unable to Call JavaScript.");
            }

            if (popup != null)
            {
                await popup.ShowAsync();
            }
        }

        private void WebGL_OnClick(object sender, RoutedEventArgs e)
        {
            this.WebViewControl.Navigate(new Uri(WebGl));
        }
    }
}
