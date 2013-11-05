using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Live;

namespace LiveConnectExample
{
    public class LiveConnectWrapper
    {
        private readonly List<String> _scopes = new List<String> { SingleSignonScope, BasicScope, CalendarsScope, SkydriveScope };

        // Basic / Core Scopes
        private const String SingleSignonScope = "wl.signin";
        private const String BasicScope = "wl.basic";
        //private const String OfflineAccessScope = "wl.offline_access";

        // Additional Personal Information
        private const String BirthdayScope = "wl.birthday";
        private const String WorkProfileScope = "wl.work_profile";
        private const String EmailScope = "wl.emails";
        private const String PostalAddressScope = "wl.postal_addresses";
        private const String PhoneNumberScope = "wl.phone_numbers";
        private const String PhotosScope = "wl.photos";

        // Additional Contact Content Access
        private const String ContactsBirthdayScope = "wl.contacts_birthday";  //Implies wl.birthday
        private const String ContactsCalendarScope = "wl.contacts_calendars"; // Implies wl.calendars
        private const String ContactsPhotosScope = "wl.contacts_photos"; // Implies wl.photos
        private const String ContactsSkydriveScope = "wl.contacts_skydrive"; // Implies wl.skydrive
        private const String ContactsCreateScope = "wl.contacts_create"; // Implies wl.skydrive

        // Calendar Access
        private const String CalendarsScope = "wl.calendars";
        private const String CalendarsUpdateScope = "wl.calendars_update"; // Implies wl.calendars
        private const String CalendarsEventCreateScope = "wl.events_create";

        // Skydrive Access
        private const String SkydriveScope = "wl.skydrive";
        private const String SkydriveUpdateScope = "wl.skydrive_update"; // Implies wl.skydrive

        // Other
        private const String ShareStatusScope = "wl.share";
        private const String ImapScope = "wl.imap";

        private LiveConnectSession _session;

        public LiveConnectWrapper()
        {
            _scopes.Add(BirthdayScope);
            _scopes.Add(WorkProfileScope);
            _scopes.Add(EmailScope);
            _scopes.Add(PostalAddressScope);
            _scopes.Add(PhoneNumberScope);
            _scopes.Add(PhotosScope);

            _scopes.Add(ContactsBirthdayScope);
            _scopes.Add(ContactsSkydriveScope);
            _scopes.Add(ContactsCalendarScope);
            _scopes.Add(ContactsCreateScope);

            _scopes.Add(CalendarsUpdateScope);
            _scopes.Add(CalendarsEventCreateScope);
        }
        
        public event EventHandler SessionChanged = delegate { };

        public Boolean IsSessionAvailable
        {
            get { return _session != null; }
        }

        private void UpdateSession(LiveConnectSession newSession)
        {
            if (_session != newSession)
            {
                _session = newSession;
                SessionChanged(this, EventArgs.Empty);
            }
        }

        public async Task<ConnectionResult> UpdateConnectionAsync()
        {
            var authClient = new LiveAuthClient();
            var sessionStatus = LiveConnectSessionStatus.Unknown;
            LiveConnectSession session = null;
            try
            {
                // IMPORTANT!  The app MUST be associated with the store, or a (otherwise unhelpful) NullReferenceException 
                // will be raised when calling the AuthClient methods.
                var loginResult = await authClient.InitializeAsync(_scopes);
                sessionStatus = loginResult.Status;
                session = loginResult.Session;
            }
            catch (NullReferenceException)
            {
                // This is basically for the sake of example only, since an actual app obtained through the store will not have
                // the problem of not having been associated with the Windows Store.
                AppNotAssociatedWithStoreError(this, EventArgs.Empty);
                return new ConnectionResult {SessionStatus = LiveConnectSessionStatus.NotConnected, CanLogout = false};
            }
            catch (LiveAuthException ex)
            {
                // TODO - handle notification/display of error information
                //throw new InvalidOperationException("An error occurred during initialization.", ex);
            }

            // Set the current instance session based on the login
            var currentSession = sessionStatus == LiveConnectSessionStatus.Connected ? session : null;
            UpdateSession(currentSession);

            var result = new ConnectionResult
                         {
                             SessionStatus = sessionStatus,
                             CanLogout = authClient.CanLogout,
                         };

            return result;
        }

        public async Task<ConnectionResult> ShowLogin()
        {
            var authClient = new LiveAuthClient();
            LiveLoginResult loginResult;
            try
            {
                // IMPORTANT!  The app MUST be associated with the store, or a (otherwise unhelpful) NullReferenceException 
                // will be raised when calling the AuthClient methods.
                loginResult = await authClient.LoginAsync(_scopes);
            }
            catch (NullReferenceException)
            {
                // This is basically for the sake of example only, since an actual app obtained through the store will not have
                // the probnlem of not having been associated with the Windows Store.
                AppNotAssociatedWithStoreError(this, EventArgs.Empty);
                return new ConnectionResult {SessionStatus = LiveConnectSessionStatus.NotConnected, CanLogout = false};
            }
            catch (LiveAuthException ex)
            {
                throw new InvalidOperationException("An error occurred during initialization.", ex);
            }

            // Set the current instance session based on the login
            var currentSession = loginResult.Status == LiveConnectSessionStatus.Connected ? loginResult.Session : null;
            UpdateSession(currentSession);

            var result = new ConnectionResult
                         {
                             SessionStatus = loginResult.Status,
                             CanLogout = authClient.CanLogout,
                         };

            return result;
        }

        public async Task<ConnectionResult> DisconnectAsync()
        {
            var authClient = new LiveAuthClient();
            try
            {
                var loginResult = await authClient.InitializeAsync();

                // Sign the user out, if he or she is connected;
                //  if not connected, skip this and just update the UI
                if (loginResult.Status == LiveConnectSessionStatus.Connected && authClient.CanLogout)
                {
                    authClient.Logout();
                    UpdateSession(null);
                }
                var result = new ConnectionResult
                {
                    SessionStatus = LiveConnectSessionStatus.NotConnected,
                    CanLogout = authClient.CanLogout,
                };

                return result;
            }
            catch (LiveConnectException ex)
            {
                // TODO - Handle exception.
                throw new InvalidOperationException("Failed to disconnect.", ex);
            }
        }

        public event EventHandler AppNotAssociatedWithStoreError = delegate { }; 

        public async Task<dynamic> GetMyProfileAsync()
        {
            // requires wl.basic scope
            return await GetUserProfileAsync("me");
        }

        public async Task<dynamic> GetUserProfileAsync(String userIdentifier)
        {
            var client = new LiveConnectClient(_session);
            var operationResult = await client.GetAsync(userIdentifier);
            dynamic result = operationResult.Result;
            return result;
        }

        public enum PictureSize
        {
            Small, // 96x96
            Medium, // 180x180
            Large // 360x360
        }

        public async Task<Uri> GetMyProfilePictureUrlAsync(PictureSize pictureSize)
        {
            return await GetUserProfilePictureUrlAsync("me", pictureSize);
        }

        public async Task<Uri> GetUserProfilePictureUrlAsync(String userIdentifier, PictureSize pictureSize)
        {
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/picture?type={1}", userIdentifier, pictureSize.ToString().ToLowerInvariant());
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var imageUrl = new Uri(result.location.ToString());
            return imageUrl;
        }

        public async Task<IEnumerable<dynamic>> GetMyContactsAsync()
        {
            // requires wl.basic scope
            var client = new LiveConnectClient(_session);
            var operationResult = await client.GetAsync("me/contacts");
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }

        public async Task<IEnumerable<dynamic>> GetMySkyDriveContentsAsync()
        {
            // requires wl.skydrive scope
            return await GetUserSkyDriveContentsAsync("me");
        }

        public async Task<IEnumerable<dynamic>> GetUserSkyDriveContentsAsync(String userIdentifier)
        {
            // requires wl.contacts_skydrive scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/skydrive/files", userIdentifier);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }


        public async Task<IEnumerable<dynamic>> GetMyCalendarsAsync()
        {
            // requires wl.calendars scope
            return await GetUserCalendarsAsync("me");
        }

        public async Task<IEnumerable<dynamic>> GetUserCalendarsAsync(String userIdentifier)
        {
            // requires wl.contacts_calendars scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/calendars", userIdentifier);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }

        public async Task<dynamic> GetContactAsync(String contactId)
        {
            // requires wl.basic scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}", contactId);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<dynamic> AddContactAsync(IDictionary<String, Object> newContact)
        {
            // requires wl.contacts_create scope
            if (newContact == null) throw new ArgumentNullException("newContact");
            var client = new LiveConnectClient(_session);
            var operationResult = await client.PostAsync("me/contacts", newContact);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<dynamic> GetCalendarAsync(String calendarId)
        {
            // requires wl.calendars scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}", calendarId);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<IEnumerable<dynamic>> GetCalendarEventsAsync(String calendarId)
        {
            // requires wl.contacts_calendars scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/events", calendarId);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }

        public async Task<dynamic> AddCalendarEventAsync(Dictionary<String, Object> newEvent)
        {
            // requires wl.calendars_update and wl.events_create scopes
            if (newEvent == null) throw new ArgumentNullException("newEvent");
            var client = new LiveConnectClient(_session);
            var operationResult = await client.PostAsync("me/events", newEvent);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<dynamic> UpdateCalendarEventAsync(String eventId, Dictionary<String, Object> updatedValues)
        {
            // requires wl.calendars_update 
            if (updatedValues == null) throw new ArgumentNullException("updatedValues");
            var client = new LiveConnectClient(_session);
            var operationResult = await client.PutAsync(eventId, updatedValues);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task DeleteCalendarEventAsync(String eventId)
        {
            // requires wl.calendars_update 
            var client = new LiveConnectClient(_session);
            await client.DeleteAsync(eventId);
        }

        public static Int32 SkyDriveItemTypeOrder(String itemType)
        {
            switch (itemType)
            {
                case "folder":
                    return 0;
                case "album":
                    return 1;
                case "audio":
                case "video":
                case "photo":
                    return 2;
                case "file":
                    return 3;
                case "notebook":
                    return 4;
                default:
                    return 5;
            }
        }
    }

    public class ConnectionResult
    {
        public LiveConnectSessionStatus SessionStatus { get; set; }
        public Boolean CanLogout { get; set; }
    }
}