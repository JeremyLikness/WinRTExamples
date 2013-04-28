// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PushNotificationHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The push notification helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PushNotificationExamples
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Windows.Data.Json;
    using Windows.Networking.PushNotifications;

    /// <summary>
    /// The push notification helper.
    /// </summary>
    public class PushNotificationHelper
    {
        /// <summary>
        /// Package security identifier
        /// </summary>
        private const string PackageSecurityIdentifier = "ms-app://s-1-15-2-2744758166-4217751632-3325801708-3408908322-2018713392-1565040368-3581600701";

        /// <summary>
        /// Client secret
        /// </summary>
        private const string ClientSecret = "zy90Ad2OxJmzjyuKcM/0QAT2rJRIDczN";

        /// <summary>
        /// Request string to get the token
        /// </summary>
        private const string TokenRequest =
            "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com";

        /// <summary>
        /// Template for a toast notification
        /// </summary>
        private const string ToastTemplate =
            "<toast>" 
            + "<visual>" 
                + "<binding template=\"ToastText01\">" 
                    + "<text id=\"1\">{0}</text>" 
                + "</binding>"
            + "</visual>" + 
            "</toast>";

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNotificationHelper"/> class.
        /// </summary>
        public PushNotificationHelper()
        {
            this.Response = "N/A";
        }

        /// <summary>
        /// Gets the push notification channel.
        /// </summary>
        public PushNotificationChannel Channel { get; private set; }

        /// <summary>
        /// Gets the access token
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// Gets the response from the service
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// The initialize task.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Initialize()
        {
            try
            {
                this.Channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                await this.GetToken();
            }
            catch (Exception ex)
            {
                this.Response = ex.Message;
            }
        }

        /// <summary>
        /// The send toast notification.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SendToastNotification(string message)
        {
            var toast = string.Format(ToastTemplate, WebUtility.HtmlEncode(message));

            var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                this.Channel.Uri)
            {
                Content =
                    new ByteArrayContent(Encoding.UTF8.GetBytes(toast))
            };

            requestMessage.Content.Headers.ContentType =
                new MediaTypeHeaderValue("text/xml");

            requestMessage.Headers.Add("X-WNS-TYPE", "wns/toast");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", this.Token);

            try
            {
                var response = await new HttpClient().SendAsync(requestMessage);
                this.Response = string.Format(
                    "{0}: {1}", response.StatusCode, await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                this.Response = ex.Message;
            }
        }

        /// <summary>
        /// The get token method.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetToken()
        {
            var requestBody = string.Format(
                TokenRequest, 
                WebUtility.UrlEncode(PackageSecurityIdentifier), 
                WebUtility.UrlEncode(ClientSecret));

            var httpBody = new StringContent(
                requestBody, 
                Encoding.UTF8, 
                "application/x-www-form-urlencoded");
            
            var client = new HttpClient();
            
            var response =
                await client.PostAsync(
                new Uri("https://login.live.com/accesstoken.srf", UriKind.Absolute), 
                httpBody);
            
            var oauthToken = JsonObject.Parse(await response.Content.ReadAsStringAsync());
            this.Token = oauthToken["access_token"].GetString();
        }
    }
}