using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LiveConnectExample.Common;
using Microsoft.Live;

namespace LiveConnectExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ContactPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        private readonly LiveConnectWrapper _liveConnectWrapper;

        private String _contactId;
        private readonly ObservableCollection<dynamic> _skydriveItems = new ObservableCollection<dynamic>();

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactPage"/> class.
        /// </summary>
        public ContactPage()
        {
            InitializeComponent();

            DefaultViewModel["RefreshCommand"] = new RelayCommand(Refresh);

            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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
            _navigationHelper.OnNavigatedTo(e);

            _contactId = e.Parameter as String;
            DefaultViewModel["IsConnected"] = _liveConnectWrapper.IsSessionAvailable;
            DefaultViewModel["ImageSource"] = new Uri("ms-appx:///Assets/Profile.png");
            DefaultViewModel["SkydriveItems"] = _skydriveItems;

            _liveConnectWrapper.SessionChanged += OnLiveConnectWrapperSessionChanged;
            await UpdateContent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);

            _liveConnectWrapper.SessionChanged -= OnLiveConnectWrapperSessionChanged;
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
                var contact = await _liveConnectWrapper.GetContactAsync(_contactId);
                DefaultViewModel["Contact"] = contact;

                var contactItems = new Dictionary<String, Object>(contact as IDictionary<String, Object>);
                contactItems.Remove("id");
                contactItems.Remove("name");

                var profileItemsList = contactItems.FlattenDynamicItems(String.Empty);
                DefaultViewModel["AdditionalDetails"] = profileItemsList.Select(x => new { x.Key, x.Value }).ToList();

                if (contact.user_id != null)
                {
                    var pictureUrl = await _liveConnectWrapper.GetUserProfilePictureUrlAsync(contact.user_id, LiveConnectWrapper.PictureSize.Medium);
                    DefaultViewModel["ImageSource"] = pictureUrl;

                    try
                    {
                        var skyDriveContents = await _liveConnectWrapper.GetUserSkyDriveContentsAsync(contact.user_id);
                        var orderedSkyDriveContents = new List<dynamic>(skyDriveContents).OrderBy(x => ((String)x.type).GetSkyDriveItemTypeOrder()).ThenBy(x => x.name);
                        _skydriveItems.Clear(); 
                        foreach (var item in orderedSkyDriveContents)
                        {
                            _skydriveItems.Add(item);
                        }

                    }
                    catch (LiveConnectException)
                    {
                        // TODO - Display error information in the UI (likely a scopes issue)
                    }
                }
            }
        }

        private async void Refresh()
        {
            await UpdateContent();
        }
    }
}
