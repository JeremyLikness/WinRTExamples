// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Main page for testing push notifications
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PushNotificationExample
{
    using System;
    using System.Diagnostics;
    using Windows.Networking.PushNotifications;
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
        private readonly NotificationRegistrationHelper _registrationHelper = new NotificationRegistrationHelper();
        private readonly SendNotificationHelper _sendNotificationHelper = new SendNotificationHelper();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Obtain the registration information
            await _registrationHelper.Initialize();
            ChannelUri.Text = _registrationHelper.Channel == null ? String.Empty : _registrationHelper.Channel.Uri;

            if (_registrationHelper.Channel != null)
            {
                _registrationHelper.Channel.PushNotificationReceived += ChannelPushNotificationReceived;
            }

            var serverResponse = await _sendNotificationHelper.Initialize();

            // Prepare the simulated server side
            Token.Text = _sendNotificationHelper.Token ?? String.Empty;
            Response.Text = serverResponse ?? String.Empty;
            Submit.IsEnabled = true;
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
            PushNotificationChannel sender, 
            PushNotificationReceivedEventArgs args)
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
        private async void Submit_OnClick(Object sender, RoutedEventArgs e)
        {
            Submit.IsEnabled = false;
            Response.Text = "(waiting...)";
            var response = await _sendNotificationHelper.SendToastNotification(NotificationText.Text);
            Submit.IsEnabled = true;
            Response.Text = response ?? String.Empty;
        }
    }
}
