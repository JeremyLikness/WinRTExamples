// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PushNotificationHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The push notification helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.System.Profile;

namespace PushNotificationExample
{
    public class NotificationRegistrationHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRegistrationHelper"/> class.
        /// </summary>
        public NotificationRegistrationHelper()
        {
        }

        /// <summary>
        /// Gets the push notification channel.
        /// </summary>
        public PushNotificationChannel Channel { get; private set; }

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
                Channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            }
            catch (Exception ex)
            {
                // Exception coming from push channel request may be generic with just an HRESULT.
                // WPN COM Error codes are listed at http://msdn.microsoft.com/en-us/library/windows/desktop/hh404142(v=vs.85).aspx
                throw new InvalidOperationException("A problem occurred during channel registration.", ex);
            }              
             
            // Once the channel has been obtained, it should be sent to the service that will be issuing the Push Notifications.
            // This step may be put off until later, in case there's metadata that should also be sent that the sevice will use 
            // to decide which of the channels that it is tracking are to be notified (eg Zip code, etc.)
            // Also, be aware that these channels do expire periodically, so the value sent to the server will need to be refreshed.
            // Often this value is cached locally in order to optimize the publication/refresh step (only send channel when it has changed.)
            var originalChannel = String.Empty;

            // Try to locate the locally cached channel value
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("NotificationChannel"))
            {
                originalChannel = localSettings.Values["NotificationChannel"].ToString();
            }

            // Check to see if the value has been updated
            // NOTE - this has been commented out in order to ensure that it cgets called every time, since the app isn't
            // actually persisting its values to disk, database, etc.  Otherwise subsequent runs of the app would fail.
            //if (Channel.Uri != originalChannel)
            {
                // Send the updated value to the "server" and cache the new current value
                UpdateStoredChannelUri(Channel.Uri);
                localSettings.Values["NotificationChannel"] = Channel.Uri;
            }
        }

        private void UpdateStoredChannelUri(String uri)
        {
            // Retrieve an encrypted, machine & app specific id so the server can know what uri goes with what system
            var hardwareToken = HardwareIdentification.GetPackageSpecificToken(null);
            var systemIdentifier = CryptographicBuffer.EncodeToBase64String(hardwareToken.Id);

            // Simulate "sending" the Channel Uri update to the server by storing it in static memory
            SendNotificationHelper.StoreChannel(systemIdentifier, uri);
        }
    }
}