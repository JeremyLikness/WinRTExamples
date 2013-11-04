using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MobileServicesExample.Common;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace MobileServicesExample
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class SubscribersPage : Page, IDialogService
    {
        private readonly SubscribersViewModel _viewModel;

        private readonly NavigationHelper _navigationHelper;

        public SubscribersViewModel ViewModel
        {
            get { return _viewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public SubscribersPage()
        {
            _viewModel = new SubscribersViewModel(this);
            _viewModel.RefreshItemsCommand.Execute(null);

            InitializeComponent();
            
            // Setup the navigation helper
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
            _navigationHelper.SaveState += HandleNavigationHelperSaveState;

            // Setup the logical page navigation components that allow
            // the page to only show one pane at a time.
            _navigationHelper.GoBackCommand = new RelayCommand(GoBack, CanGoBack);
            ItemListView.SelectionChanged += HandleItemListViewSelectionChanged;

            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += HandleWindowSizeChanged;
            InvalidateVisualState();
        }

        private void HandleItemListViewSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (UsingLogicalPageNavigation())
            {
                _navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
            }
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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void HandleNavigationHelperSaveState(Object sender, SaveStateEventArgs e)
        {
        }

        private async void LoadContent()
        {
            var client = App.WinRTByExampleBookClient;
            var sample = await client.InvokeApiAsync<Sample>("sample");
            System.Diagnostics.Debug.WriteLine(sample.Count);
        }

        public class Sample
        {
            public Int32 Count { get; set; }
        }

        #region Logical page navigation

        // The split page isdesigned so that when the Window does have enough space to show
        // both the list and the dteails, only one pane will be shown at at time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <returns>True if the window should show act as one logical page, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Invoked with the Window changes size
        /// </summary>
        /// <param name="sender">The current Window</param>
        /// <param name="e">Event data that describes the new size of the Window</param>
        private void HandleWindowSizeChanged(Object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        private void ItemListView_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (UsingLogicalPageNavigation()) InvalidateVisualState();
        }

        private bool CanGoBack()
        {
            return UsingLogicalPageNavigation() && ItemListView.SelectedItem != null 
                || _navigationHelper.CanGoBack();
        }

        private void GoBack()
        {
            if (UsingLogicalPageNavigation() && ItemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                ItemListView.SelectedItem = null;
            }
            else
            {
                _navigationHelper.GoBack();
            }
        }

        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            _navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        private string DetermineVisualState()
        {
            if (!UsingLogicalPageNavigation())
                return "PrimaryView";

            // Update the back button's enabled state when the view state changes
            var logicalPageBack = UsingLogicalPageNavigation() && ItemListView.SelectedItem != null;

            return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
        }

        #endregion

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

        public async void ShowMessageBox(String content, String title)
        {
            var messageDialog = new MessageDialog(content, title);
            await messageDialog.ShowAsync();
        }

        public async void ShowError(String content)
        {
            var messageDialog = new MessageDialog(content, "Error");
            await messageDialog.ShowAsync();
        }
    }

    public interface IDialogService
    {
        void ShowMessageBox(String content, String title);

        void ShowError(String content);
    }
}
