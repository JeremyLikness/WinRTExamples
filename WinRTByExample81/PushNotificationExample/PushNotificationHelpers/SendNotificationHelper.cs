// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PushNotificationHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The push notification helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PushNotificationExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Data.Json;

    /// <summary>
    /// Uttility class to encapsulate the functionality of sending push notifications
    /// </summary>
    /// <remarks>
    /// This code normally resides only on the server, and is included here in the client for the purposes of example only.
    /// Care should be taken to protect your app's Package SID and Client Secret values
    /// </remarks>
    public class SendNotificationHelper
    {
        /// <summary>
        /// Package security identifier
        /// </summary>
        private const String PackageSecurityIdentifier = "YOUR_PACKAGE_SID_HERE";

        /// <summary>
        /// Client secret
        /// </summary>
        private const String ClientSecret = "YOUR_CLIENT_SECRET_HERE";

        /// <summary>
        /// Request string to get the token
        /// </summary>
        private const String TokenRequest =
            "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com";

        /// <summary>
        /// Template for a toast notification
        /// </summary>
        private const String ToastTemplate =
            "<toast>" 
            + "<visual>" 
                + "<binding template=\"ToastText01\">" 
                    + "<text id=\"1\">{0}</text>" 
                + "</binding>"
            + "</visual>" + 
            "</toast>";

        /// <summary>
        /// Initializes a new instance of the <see cref="SendNotificationHelper"/> class.
        /// </summary>
        public SendNotificationHelper()
        {
        }

        /// <summary>
        /// Gets the access token
        /// </summary>
        public String Token { get; private set; }

        /// <summary>
        /// The initialize task.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<String> Initialize()
        {
            String response;
            try
            {
                await GetToken();
                response = "Successfully Retrieved Token";
            }
            catch (HttpRequestException ex)
            {
                const String message = 
                "There was a problem obtaining a valid token for the request.  Check to ensure the has been associated " +
                "with the Windows Store and the Package SID and Client Secret have been properly entered in the app code.";
                var responseMessage = String.Format("{0}\n{1}", message, ex.Message);
                response = responseMessage;
            }
            return response;
        }

        /// <summary>
        /// The send toast notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public async Task<String> SendToastNotification(String message)
        {
            var toast = String.Format(ToastTemplate, WebUtility.HtmlEncode(message));

            // Simulate retrieveing the endpoints that are "interested" in this particular message
            var responseBuilder = new StringBuilder();
            foreach (var channelRecord in NotificationChannels)
            {
                var requestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    channelRecord.ChannelUri)
                                     {
                                         Content =
                                             new ByteArrayContent(Encoding.UTF8.GetBytes(toast))
                                     };

                requestMessage.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("text/xml");

                requestMessage.Headers.Add("X-WNS-TYPE", "wns/toast");

                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", Token);
                
                var response = await new HttpClient().SendAsync(requestMessage);
                responseBuilder.AppendFormat("{0}: {1}", response.StatusCode, 
                    await response.Content.ReadAsStringAsync());
            }
            return responseBuilder.ToString();
        }

        /// <summary>
        /// The get token method.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task GetToken()
        {
            var requestBody = String.Format(
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

            // Throws an ??? exception if a successful respons is not returned.
            response.EnsureSuccessStatusCode();
            
            var oauthToken = JsonObject.Parse(await response.Content.ReadAsStringAsync());
            Token = oauthToken["access_token"].GetString();
        }

        private static readonly List<NotificationChannelRecord> NotificationChannels = 
            new List<NotificationChannelRecord>();

        public static void StoreChannel(String systemIdentifier, String uri)
        {
            var registration = NotificationChannels.FirstOrDefault(x => x.SystemIdentifier == systemIdentifier);
            if (registration == null)
            {
                // No entry for this system is in the "database" - add one
                registration = new NotificationChannelRecord
                               {
                                   SystemIdentifier = systemIdentifier,
                                   ChannelUri = uri
                               };
                NotificationChannels.Add(registration);
            }
            else
            {
                // An entry for this system is already in the "database" - replace it with the possibly updated value
                registration.ChannelUri = uri;
            }
        }

        private class NotificationChannelRecord
        {
            public String SystemIdentifier { get; set; }
            public String ChannelUri { get; set; }
        }
    }
}