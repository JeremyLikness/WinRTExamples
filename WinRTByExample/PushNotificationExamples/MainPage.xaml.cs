// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Main page for testing push notifications
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PushNotificationExamples
{
    using System.Diagnostics;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Push notification helper
        /// </summary>
        private PushNotificationHelper helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.helper != null)
            {
                return;
            }

            this.helper = new PushNotificationHelper();
            await this.helper.Initialize();
            this.ChannelUri.Text = this.helper.Channel == null ? string.Empty : this.helper.Channel.Uri;
            this.Token.Text = this.helper.Token ?? string.Empty;
            this.Response.Text = this.helper.Response ?? string.Empty;
            Submit.IsEnabled = true;

            if (this.helper.Channel != null)
            {
                this.helper.Channel.PushNotificationReceived += ChannelPushNotificationReceived;
            }
        }

        /// <summary>
        /// The channel_ push notification received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void ChannelPushNotificationReceived(
            Windows.Networking.PushNotifications.PushNotificationChannel sender, 
            Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs args)
        {
            Debug.WriteLine("A push notification was received.");            
        }

        /// <summary>
        /// The submit_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void Submit_OnClick(object sender, RoutedEventArgs e)
        {
            Submit.IsEnabled = false;
            Response.Text = "(waiting...)";
            await this.helper.SendToastNotification(NotificationText.Text);
            Submit.IsEnabled = true;
            Response.Text = this.helper.Response ?? string.Empty;
        }
    }
}
