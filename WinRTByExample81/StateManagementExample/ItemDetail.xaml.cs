using StateManagementExample.Common;
using System;

namespace StateManagementExample
{
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ItemDetail
    {
        private Item item;
        private readonly NavigationHelper navigationHelper;

        private const string ItemTextKey = "ItemKey";
     
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ItemDetail()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelperLoadState;
            this.navigationHelper.SaveState += this.NavigationHelperSaveState;
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
        private void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
            this.ItemEditControl.Text = string.Empty;

            if (e.PageState != null && e.PageState.ContainsKey(ItemTextKey))
            {
                this.ItemEditControl.Text = e.PageState[ItemTextKey].ToString();
            }

            var text = e.NavigationParameter.ToString();
            int id;

            this.item = int.TryParse(text, out id) ? (id == 0 ? new Item() : App.ItemById(id)) : new Item();

            if (this.item != null && string.IsNullOrWhiteSpace(this.ItemEditControl.Text))
            {
                this.ItemEditControl.Text = item.Text ?? string.Empty;
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
            if (App.StateManagement)
            {
                e.PageState[ItemTextKey] = ItemEditControl.Text;
            }
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void SaveOnClick(object sender, RoutedEventArgs e)
        {
            var newText = this.ItemEditControl.Text;
            var isNew = string.IsNullOrEmpty(this.item.Text);
            if (string.IsNullOrWhiteSpace(newText))
            {
                var dialog = new MessageDialog("You must enter text.", "Validation Error");
                await dialog.ShowAsync();
            }
            else
            {
                this.item.Text = newText;
                if (isNew)
                {
                    App.ItemList.Add(this.item);
                }
                this.Frame.Navigate(typeof(MainPage), this.item.Id);
            }
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
