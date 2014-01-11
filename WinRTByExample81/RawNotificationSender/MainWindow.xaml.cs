using System;
using System.Windows;

namespace RawNotificationSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string Secret = "Your App Secret";
        private const string Sid = "ms-app://Your App Sid";
        private const string NotificationType = "wns/raw";
        private const string ContentType = "application/octet-stream";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendClick(object sender, RoutedEventArgs e)
        {
            SendButton.IsEnabled = false;
            string message;
            Uri uri;
            if (Uri.TryCreate(ChannelUri.Text, UriKind.Absolute, out uri))
            {
                if (string.IsNullOrWhiteSpace(Message.Text))
                {
                    message = "You must enter a message";
                }
                else
                {
                    try
                    {
                        StatusText.Text = "Sending notification...";
                        message =
                            await
                            PushNotificationHelper.NotificationManager.PostToWns(
                                Secret,
                                Sid,
                                ChannelUri.Text,
                                Message.Text,
                                NotificationType,
                                ContentType);
                    }
                    catch (Exception ex)
                    {
                        message = string.Format("Error: {0}", ex.Message);
                    }
                }
            }
            else
            {
                message = "Invalid uri";
            }

            StatusText.Text = message;
            SendButton.IsEnabled = true;
        }
    }
}
