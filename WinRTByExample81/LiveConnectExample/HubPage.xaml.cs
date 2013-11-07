using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using LiveConnectExample.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Live;

namespace LiveConnectExample
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly IDialogService _dialogService = new DialogService();
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        private readonly LiveConnectWrapper _liveConnectWrapper;

        private readonly ObservableCollection<dynamic> _contacts = new ObservableCollection<dynamic>();
        private readonly ObservableCollection<dynamic> _skydriveItems = new ObservableCollection<dynamic>();
        private readonly ObservableCollection<dynamic> _calendars = new ObservableCollection<dynamic>();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public HubPage()
        {
            InitializeComponent();

            DefaultViewModel["RefreshItemsCommand"] = new RelayCommand(RefreshItems);
            DefaultViewModel["AddContactCommand"]= new RelayCommand(AddContact);
            DefaultViewModel["SaveContactBeingAddedCommand"] = new RelayCommand(SaveContactBeingEdited);

            _liveConnectWrapper = ((App) Application.Current).LiveConnectWrapper;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
}

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;
            DefaultViewModel["ImageSource"] = new Uri("ms-appx:///Assets/Profile.png");
            DefaultViewModel["Contacts"] = _contacts;
            DefaultViewModel["SkydriveItems"] = _skydriveItems;
            DefaultViewModel["Calendars"] = _calendars;

            base.OnNavigatedTo(e);
            _liveConnectWrapper.SessionChanged += OnLiveConnectWrapperSessionChanged;
            await _liveConnectWrapper.UpdateConnectionAsync();
            await UpdateContent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _liveConnectWrapper.SessionChanged -= OnLiveConnectWrapperSessionChanged;
            base.OnNavigatedFrom(e);
        }

        #endregion

        private async void OnLiveConnectWrapperSessionChanged(Object sender, EventArgs eventArgs)
        {
            await UpdateContent();
        }

        private async Task UpdateContent()
        {
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;

            if (_liveConnectWrapper.IsSessionAvailable)
            {
                var myProfile = await _liveConnectWrapper.GetMyProfileAsync();
                DefaultViewModel["Me"] = myProfile;
                DefaultViewModel["ImageSource"] =
                    await _liveConnectWrapper.GetMyProfilePictureUrlAsync(LiveConnectWrapper.ProfilePictureSize.Medium);

                var profileItems = new Dictionary<String, Object>(myProfile as IDictionary<String, Object>);
                profileItems.Remove("id");
                profileItems.Remove("name");
                profileItems.Remove("first_name");
                profileItems.Remove("last_name");
                profileItems.Remove("link");

                var profileItemsList = profileItems.FlattenDynamicItems(String.Empty);
                DefaultViewModel["AdditionalDetails"] = profileItemsList.Select(x => new { x.Key, x.Value }).ToList();

                var myContacts = await _liveConnectWrapper.GetMyContactsAsync();
                var orderedContacts = myContacts.OrderBy(x => x.last_name).ThenBy(x => x.first_name);
                _contacts.Clear();
                foreach (var contact in orderedContacts)
                {
                    _contacts.Add(contact);
                }

                //var users = _contacts.Where(x => x.user_id != null).ToList();

                var mySkyDriveContents = await _liveConnectWrapper.GetMySkyDriveContentsAsync();
                var orderedSkyDriveContents = mySkyDriveContents.OrderBy(x => ((String)x.type).GetSkyDriveItemTypeOrder()).ThenBy(x => x.name);
                _skydriveItems.Clear();
                foreach (var item in orderedSkyDriveContents)
                {
                    _skydriveItems.Add(item);
                }

                var myCalendars = await _liveConnectWrapper.GetMyCalendarsAsync();
                var orderedCalendars = myCalendars.OrderBy(x => x.name);
                _calendars.Clear(); 
                foreach (var item in orderedCalendars)
                {
                    _calendars.Add(item);
                }
            }
        }

        private void HandleContactClicked(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var contactId = ((dynamic)e.ClickedItem).id;
            Frame.Navigate(typeof(ContactPage), contactId);
        }

        private void HandleCalendarClicked(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var calendarId = ((dynamic)e.ClickedItem).id;
            Frame.Navigate(typeof(CalendarPage), calendarId);
        }

        private void HandleSkyDriveItemClicked(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var skydriveItem = ((dynamic)e.ClickedItem);
            String itemType = skydriveItem.type.ToString().ToLowerInvariant();
            if ("album".Equals(itemType) || "folder".Equals(itemType))
            {
                String containerId = skydriveItem.id;
                Frame.Navigate(typeof (SkyDriveItemsPage), containerId);
            }
            else
            {
                _dialogService.ShowError("Only folders and albums can show content.");
            }
        }
        
        private async void RefreshItems()
        {
            await UpdateContent();
        }

        private void AddContact()
        {
            DefaultViewModel["ContactBeingEdited"] = new Contact();
        }

        private async void SaveContactBeingEdited()
        {
            var editedContact = (Contact)DefaultViewModel["ContactBeingEdited"];

            var newContact = new Dictionary<String, Object>
                                 {
                                     {"first_name", editedContact.FirstName},
                                     {"last_name",  editedContact.LastName},
                                     {"emails", new Dictionary<String, Object>
                                              {
                                                  {"preferred", editedContact.PreferredEmail}
                                              }}
                                 };
            try
            {
                var savedContact = await _liveConnectWrapper.AddContactAsync(newContact);
                if (savedContact != null)
                {
                    _contacts.Add(savedContact);
                }
            }
            catch (LiveConnectException ex)
            {
                _dialogService.ShowError(ex.Message);
            }
        }
    }
}
