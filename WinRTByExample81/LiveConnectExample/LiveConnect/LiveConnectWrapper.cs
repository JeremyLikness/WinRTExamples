using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Live;

namespace LiveConnectExample
{
    public class LiveConnectWrapper
    {
        #region Scope Contant Declarations

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

        // Additional Contact Content Access
        private const String ContactsBirthdayScope = "wl.contacts_birthday";  //Implies wl.birthday
        private const String ContactsCalendarScope = "wl.contacts_calendars"; // Implies wl.calendars
        private const String ContactsSkydriveScope = "wl.contacts_skydrive"; // Implies wl.skydrive
        private const String ContactsPhotosScope = "wl.contacts_photos"; // Implies wl.photos
        private const String ContactsCreateScope = "wl.contacts_create";

        // Calendar Access
        private const String CalendarsScope = "wl.calendars";
        private const String CalendarsUpdateScope = "wl.calendars_update"; // Implies wl.calendars
        private const String CalendarsEventCreateScope = "wl.events_create";

        // Skydrive Access
        private const String SkydriveScope = "wl.skydrive";
        private const String SkydriveUpdateScope = "wl.skydrive_update"; // Implies wl.skydrive
        private const String PhotosScope = "wl.photos"; // Subset of skydrive - allows read-only access to photos, videos, audio, albums

        // Other Extended Scopes
        private const String ShareStatusScope = "wl.share";
        private const String ImapScope = "wl.imap";

        // Special user id substitution "Me" value
        private const String Me = "me";

        #endregion

        #region Fields

        private LiveConnectSession _session;
        private readonly List<String> _scopes;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveConnectWrapper"/> class.
        /// </summary>
        public LiveConnectWrapper()
        {
            // Establish the scopes
            _scopes = new List<String>
                      {
                          SingleSignonScope,
                          BasicScope,
                          CalendarsScope,
                          SkydriveScope,

                          BirthdayScope,
                          WorkProfileScope,
                          EmailScope,
                          PostalAddressScope,
                          PhoneNumberScope,
                          //PhotosScope,

                          ContactsBirthdayScope,
                          ContactsSkydriveScope,
                          ContactsCalendarScope,
                          ContactsCreateScope,

                          CalendarsUpdateScope,
                          CalendarsEventCreateScope,

                          SkydriveUpdateScope
                      };
        } 

        #endregion

        #region Session Management

        /// <summary>
        /// Occurs when the underlying <see cref="LiveConnectSession"/> is changed.
        /// </summary>
        public event EventHandler SessionChanged = delegate { };

        /// <summary>
        /// Gets a value indicating whether a session is available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a session is available; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsSessionAvailable
        {
            get { return _session != null; }
        }

        private void UpdateSession(LiveConnectSession newSession)
        {
            // Update the session and raise the correspoding event
            if (_session != newSession)
            {
                _session = newSession;
                SessionChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Authentication

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
                return new ConnectionResult { SessionStatus = LiveConnectSessionStatus.NotConnected, CanLogout = false };
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
                return new ConnectionResult { SessionStatus = LiveConnectSessionStatus.NotConnected, CanLogout = false };
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
        #endregion

        #region Profile Information

        public async Task<dynamic> GetMyProfileAsync()
        {
            // requires wl.basic scope
            return await GetUserProfileAsync(Me);
        }

        public async Task<dynamic> GetUserProfileAsync(String userIdentifier)
        {
            var client = new LiveConnectClient(_session);
            var operationResult = await client.GetAsync(userIdentifier);
            dynamic result = operationResult.Result;
            return result;
        }

        public enum ProfilePictureSize
        {
            Small, // 96x96
            Medium, // 180x180
            Large // 360x360
        }

        public async Task<Uri> GetMyProfilePictureUrlAsync(ProfilePictureSize pictureSize)
        {
            return await GetUserProfilePictureUrlAsync(Me, pictureSize);
        }

        public async Task<Uri> GetUserProfilePictureUrlAsync(String userIdentifier, ProfilePictureSize pictureSize)
        {
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/picture?type={1}", userIdentifier, pictureSize.ToString().ToLowerInvariant());
            var operationResult = await client.GetAsync(path);
            //var operationResult = await client.GetAsync("me/picture?type=large");
            dynamic result = operationResult.Result;
            var imageUrl = new Uri(result.location.ToString());
            return imageUrl;
        }

        #endregion

        #region Contacts

        public async Task<IEnumerable<dynamic>> GetMyContactsAsync()
        {
            // requires wl.basic scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/contacts", Me);
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
            var path = String.Format("{0}/contacts", Me);
            var operationResult = await client.PostAsync(path, newContact);
            dynamic result = operationResult.Result;
            return result;

            // Example:
            //var newContact = new Dictionary<String, Object>
            //                     {
            //                         {"first_name", "Joe"},
            //                         {"last_name", "Smith"},
            //                     };
            //var client = new LiveConnectClient(_session);
            //var operationResult = await client.PostAsync("me/contacts", newContact);
            //dynamic result = operationResult.Result;
            //return result;
        }

        #endregion

        #region Calendar and Calendar Events

        public async Task<IEnumerable<dynamic>> GetMyCalendarsAsync()
        {
            // requires wl.calendars scope
            return await GetUserCalendarsAsync(Me);
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

            // Example
            //var operationResult = await client.GetAsync("me/calendars");
            //dynamic result = operationResult.Result;
            //var resultList = new List<dynamic>(result.data);
            //return resultList;
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

        public const String DateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        public async Task<IEnumerable<dynamic>> GetCalendarEventsAsync(String calendarId)
        {
            var path = String.Format("{0}/events", calendarId);
            return await GetCalendarEventsInternalAsync(path);
        }

        public async Task<IEnumerable<dynamic>> GetCalendarEventsAsync(String calendarId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var startTimeText = startTime.ToString(DateTimeFormatString);
            var endTimeText = endTime.ToString(DateTimeFormatString);
            var path = String.Format("{0}/events?start_time={1}&end_time={2}", calendarId, startTimeText, endTimeText);
            return await GetCalendarEventsInternalAsync(path);

            // Example
            //// First, convert the requested start and end values to 
            //// the date/time format that the Live Connect API requires
            //const String DateTimeFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
            //var startTimeText = startTime.ToString(DateTimeFormatString);
            //var endTimeText = endTime.ToString(DateTimeFormatString);
            
            //// Build the path for the given calendar id and timeframe
            //var path = String.Format("{0}/events?start_time={1}&end_time={2}", 
            //    calendarId, startTimeText, endTimeText);

            //// Make the request and return the results
            //var client = new LiveConnectClient(_session);
            //var operationResult = await client.GetAsync(path);
            //dynamic result = operationResult.Result;
            //var resultList = new List<dynamic>(result.data);
            //return resultList;
        }

        private async Task<IEnumerable<dynamic>> GetCalendarEventsInternalAsync(String path)
        {
            // requires wl.contacts scope
            var client = new LiveConnectClient(_session);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }


        public async Task<dynamic> AddCalendarEventAsync(String calendarId, Dictionary<String, Object> newEvent)
        {
            // requires wl.events_create scopes
            if (newEvent == null) throw new ArgumentNullException("newEvent");
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/events", calendarId);
            var operationResult = await client.PostAsync(path, newEvent);
            dynamic result = operationResult.Result;
            return result;

            // Example
            //var newEvent = new Dictionary<String, Object>
            //                    {
            //                        {"name", eventName},
            //                        {"description", eventDescription},
            //                        {"start_time", startTimeDate},
            //                        {"end_time", endTimeDate},
            //                        {"location", locationText},
            //                        {"is_all_day_event", false},
            //                        {"availability", "busy"},
            //                        {"visibility", "public"}
            //                    };

            //var client = new LiveConnectClient(_session);
            //var path = String.Format("{0}/events", calendarId);
            //var operationResult = await client.PostAsync(path, newEvent);
            //dynamic result = operationResult.Result;
            //return result;
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

        #endregion

        #region SkyDrive

        public async Task<IEnumerable<dynamic>> GetMySkyDriveContentsAsync()
        {
            // requires wl.skydrive scope
            return await GetUserSkyDriveContentsAsync(Me);
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

            // Example
            //var client = new LiveConnectClient(_session);
            //var operationResult = await client.GetAsync("me/skydrive/files");
            //dynamic result = operationResult.Result;
            //var resultList = new List<dynamic>(result.data);
            //return resultList;
        }

        public async Task<IEnumerable<dynamic>> GetSkydriveContentsAsync(String skyDriveContainerItemId)
        {
            // requires wl.skydrive or wl.contacts_skydrive scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/files", skyDriveContainerItemId);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var resultList = new List<dynamic>(result.data);
            return resultList;
        }

        public enum SpecialFolder
        {
            CameraRoll,
            Documents,
            Pictures,
            Public
        }

        private const String CameraRollFolderName = "camera_roll";
        private const String DocumentsFolderName = "my_documents";
        private const String PicturesFolderName = "my_photos";
        private const String PublicFolderName = "public_documents";

        public async Task<dynamic> GetMySkyDriveSpecialFolderAsync(SpecialFolder specialFolder)
        {
            // requires wl.skydrive scope
            return await GetUserSkyDriveSpecialFolderAsync(Me, specialFolder);
        }

        public async Task<IEnumerable<dynamic>> GetUserSkyDriveSpecialFolderAsync(String userIdentifier, SpecialFolder specialFolder)
        {
            // requires wl.contacts_skydrive scope
            var client = new LiveConnectClient(_session);
            String specialFolderName;
            switch (specialFolder)
            {
                case SpecialFolder.CameraRoll:
                    specialFolderName = CameraRollFolderName;
                    break;
                case SpecialFolder.Documents:
                    specialFolderName = DocumentsFolderName;
                    break;
                case SpecialFolder.Pictures:
                    specialFolderName = PicturesFolderName;
                    break;
                case SpecialFolder.Public:
                    specialFolderName = PublicFolderName;
                    break;
                default:
                    throw new InvalidOperationException("Unknown folder type requested");
            }

            var path = String.Format("{0}/skydrive/{1}", userIdentifier, specialFolderName);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<dynamic> GetSkyDriveItemAsync(String skyDriveItemId)
        {
            // requires wl.skydrive scope
            var client = new LiveConnectClient(_session);
            var operationResult = await client.GetAsync(skyDriveItemId);
            dynamic result = operationResult.Result;
            return result;
        }

        public enum PictureSize
        {
            Thumbnail,
            Small,
            Album,
            Normal,
            Full
        }
        
        public async Task<Uri> GetSkydriveItemPictureAsync(String skyDriveItemId, PictureSize pictureSize)
        {
            // requires wl.skydrive scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/picture?type={1}", skyDriveItemId, pictureSize.ToString().ToLowerInvariant());
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var imageUrl = new Uri(result.location.ToString());
            return imageUrl;
        }

        public async Task<Uri> GetSkydriveItemShareLinkUrlAsync(String skyDriveItemId, Boolean isReadOnly)
        {
            // requires wl.skydrive scope
            var client = new LiveConnectClient(_session);
            var path = isReadOnly
                ? String.Format("{0}/shared_read_link", skyDriveItemId)
                : String.Format("{0}/shared_edit_link", skyDriveItemId);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var linkUrl = new Uri(result.link.ToString());
            return linkUrl;
        }

        public async Task<dynamic> RenameSkyDriveItemAsync(String skyDriveItemId, String itemNewName)
        {
            // requires wl.skydrive_update scope
            var client = new LiveConnectClient(_session);
            var updateData = new Dictionary<String, Object> { { "name", itemNewName } };
            var operationResult = await client.PutAsync(skyDriveItemId, updateData);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task DeleteSkyDriveItemAsync(String skyDriveItemId)
        {
            // requires wl.skydrive_update scope
            var client = new LiveConnectClient(_session);
            await client.DeleteAsync(skyDriveItemId);
        }

        public const String DefaultNewFolderName = "New Folder";
        public async Task<dynamic> CreateSkyDriveFolderAsync(String skyDriveContainingFolderId, String folderName)
        {
            // requires wl.skydrive_update scope
            var client = new LiveConnectClient(_session);
            var folderData = new Dictionary<String, Object> { { "name", folderName } };
            var operationResult = await client.PostAsync(skyDriveContainingFolderId, folderData);
            dynamic result = operationResult.Result;
            return result;
        }

        public async Task<LiveDownloadOperationResult> StartBackgroundDownloadAsync(String skyDriveItemId, StorageFile file, CancellationToken cancellationToken, IProgress<LiveOperationProgress> progressHandler)
        {
            // requires wl.skydrive scope
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/content", skyDriveItemId);
            var result = await client.BackgroundDownloadAsync(path, file, cancellationToken, progressHandler);
            return result;
        }

        // Example
        //private async Task<LiveDownloadOperationResult> DownloadExample(
        //    String skyDriveItemId, 
        //    IStorageFile downloadFile)
        //{
        //    var client = new LiveConnectClient(_session);
        //    var path = String.Format("{0}/content", skyDriveItemId);
        //    var progressHandler =
        //        new Progress<LiveOperationProgress>(ShowProgress); 
        //    var result = await client.BackgroundDownloadAsync(
        //        path,
        //        downloadFile,
        //        _cancellationTokenSource.Token,
        //        progressHandler);
        //    return result;
        //    // result.File
        //    // result.GetRandomAccessStreamAsync()
        //    // result.Stream
        //}

        //private void ShowProgress(LiveOperationProgress liveOperationProgress)
        //{
        //    // Display progress UI, making sure to check for thread access
        //    UpdateUserInterfaceProgress(
        //        liveOperationProgress.BytesTransferred,
        //        liveOperationProgress.TotalBytes,
        //        liveOperationProgress.ProgressPercentage);
        //}

        //private void CancelDownload(CancellationTokenSource tokenSource)
        //{
        //    // Respond to UI request to cancel the download
        //    tokenSource.Cancel();
        //}

        public async Task<dynamic> StartBackgroundUploadAsync(String skyDriveItemId, String uploadFileName, StorageFile fileToUpload, CancellationToken cancellationToken, Progress<LiveOperationProgress> progressHandler)
        {
            // requires wl.skydrive_update scope
            var client = new LiveConnectClient(_session);
            var result = await client.BackgroundUploadAsync(skyDriveItemId, uploadFileName, fileToUpload, OverwriteOption.Rename, cancellationToken, progressHandler);
            return result.Result;
        }

        // Example
        //private async void UploadExample(
        //    IStorageFile fileToUpload, 
        //    String uploadFolderId)
        //{
        //    var fileInfo = await fileToUpload.GetBasicPropertiesAsync();

        //    var client = new LiveConnectClient(_session);
        //    var path = String.Format("{0}/skydrive/quota", Me);
        //    var quotaOperationResult = await client.GetAsync(path);
        //    dynamic quotaResult = quotaOperationResult.Result;
        //    if ((UInt64) quotaResult.available < fileInfo.Size)
        //    {
        //        // Handle quota error - not enough room available
        //    }

        //    // requires wl.skydrive_update scope
        //    var operationResult = await client.BackgroundUploadAsync(
        //        uploadFolderId, 
        //        fileToUpload.Name, 
        //        fileToUpload, 
        //        OverwriteOption.Rename, 
        //        cancellationToken, 
        //        progressHandler);
        //    dynamic result = operationResult.Result;
        //    var uploadedItemId = result.id;
        //}

        public class SkyDriveQuota
        {
            public Int64 Quota { get; set; }
            public Int64 Available{ get; set; }

        }

        public async Task<SkyDriveQuota> GetUserSkyDriveQuotaAsync()
        {
            var client = new LiveConnectClient(_session);
            var path = String.Format("{0}/skydrive/quota", Me);
            var operationResult = await client.GetAsync(path);
            dynamic result = operationResult.Result;
            var quota = new SkyDriveQuota
                        {
                            Quota = result.quota,
                            Available = result.available
                        };
            return quota;
        }

        #endregion

    }

    public class ConnectionResult
    {
        public LiveConnectSessionStatus SessionStatus { get; set; }
        public Boolean CanLogout { get; set; }
    }
}