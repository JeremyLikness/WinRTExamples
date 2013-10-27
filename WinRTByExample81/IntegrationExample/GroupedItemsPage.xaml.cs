using System;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using IntegrationExample.Common;

namespace IntegrationExample
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public GroupedItemsPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
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
        private void HandleNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
            var sampleDataGroups = Application.Current.GetSampleData().Groups;
            DefaultViewModel["Groups"] = sampleDataGroups;
        }

        ///// <summary>
        ///// Invoked when a group header is clicked.
        ///// </summary>
        ///// <param name="sender">The Button used as a group header for the selected group.</param>
        ///// <param name="e">Event data that describes how the click was initiated.</param>
        //void Header_Click(Object sender, RoutedEventArgs e)
        //{
        //    // Determine what group the Button instance represents
        //    var group = ((FrameworkElement)sender).DataContext;

        //    // Navigate to the appropriate destination page, configuring the new page
        //    // by passing required information as a navigation parameter
        //    Frame.Navigate(typeof(GroupDetailPage), ((SampleContactGroup)group).Key);
        //}

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(Object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Contact)e.ClickedItem).Id;
            Frame.Navigate(typeof(ContactDetailPage), itemId);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void HandleAddContactClick(Object sender, RoutedEventArgs e)
        {
            var contactPicker = new ContactPicker
                                {
                                    SelectionMode = ContactSelectionMode.Contacts,
                                    //SelectionMode = ContactSelectionMode.Fields,
                                    //DesiredFieldsWithContactFieldType = {ContactFieldType.Email}
                                };
            var contacts = await contactPicker.PickContactsAsync();
            foreach (var contact in contacts)
            {
                Application.Current.GetSampleData().AddContact(contact);
            }
        }
    }
}