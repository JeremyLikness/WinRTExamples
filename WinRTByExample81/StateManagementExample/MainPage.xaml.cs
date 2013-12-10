namespace StateManagementExample
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using Windows.Storage.Streams;

    using StateManagementExample.Common;
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly NavigationHelper navigationHelper;

        private string searchTerm = string.Empty;
        
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
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
            if (!App.StateManagement)
            {
                return;
            }

            var text = e.NavigationParameter.ToString();
            int id;

            if (int.TryParse(text, out id))
            {
                App.CurrentItem = App.ItemById(id);
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
            // to-do: anything special here             
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ItemListControl.ItemsSource = App.ItemList;
            ToggleControl.IsOn = App.StateManagement;
            App.CurrentItem = ItemListControl.SelectedItem as Item; 

            navigationHelper.OnNavigatedTo(e);
            
            if (App.CurrentItem != null)
            {
                this.ItemListControl.SelectedIndex = App.ItemList.IndexOf(App.CurrentItem);                
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region Add and Select (Navigation)

        private void AddOnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemDetail), 0);
        }

        private void ItemListControlOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.CurrentItem = this.ItemListControl.SelectedItem as Item;
           
            if (App.CurrentItem != null)
            {
                this.Frame.Navigate(typeof(ItemDetail), App.CurrentItem.Id);
            }
        }

        private void ToggleControlOnToggled(object sender, RoutedEventArgs e)
        {
            App.StateManagement = this.ToggleControl.IsOn; 
        }

        #endregion 

        #region Search Functionality 

        private void SearchBoxControlOnQuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            this.searchTerm = args.QueryText;
            this.ItemListControl.ItemsSource = string.IsNullOrWhiteSpace(this.searchTerm)
                                                   ? App.ItemList
                                                   : new ObservableCollection<Item>(App.ItemList.Where(i => i.Text.StartsWith(this.searchTerm)).ToList());
        }

        private void SearchBoxControlOnSuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.QueryText))
            {
                return;
            }
            var collection = args.Request.SearchSuggestionCollection;
            var results =
                App.ItemList.Where(i => i.Text.StartsWith(args.QueryText, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            
            bool separator = false;

            foreach (var item in results)
            {
                separator = true;
                collection.AppendQuerySuggestion(item.Text);
            }

            if (separator)
            {
                collection.AppendSearchSeparator("Matches");
            }

            if (args.QueryText.Length > 2)
            {
                foreach (var item in results)
                {
                    var uri = new Uri("ms-appx:///Assets/StoreLogo.scale-100.png");
                    var image = RandomAccessStreamReference.CreateFromUri(uri);
                    collection.AppendResultSuggestion(
                        item.Id.ToString(),
                        item.Text,
                        item.Id.ToString(),
                        image,
                        item.Text);
                }
            }
        }

        private void SearchBoxControlOnResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
        {
            int id;
            if (string.IsNullOrWhiteSpace(args.Tag) || !int.TryParse(args.Tag, out id))
            {
                return;
            }
            App.CurrentItem = App.ItemById(id);
            if (App.CurrentItem != null)
            {
                this.Frame.Navigate(typeof(ItemDetail), App.CurrentItem.Id);
            }
        }

        private void SearchBoxControlOnQueryChanged(SearchBox sender, SearchBoxQueryChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.QueryText) && !string.IsNullOrWhiteSpace(args.QueryText))
            {
                this.ItemListControl.ItemsSource = App.ItemList;
            }
        }

        #endregion

    }
}
