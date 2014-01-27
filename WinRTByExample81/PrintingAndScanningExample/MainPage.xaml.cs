using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using PrintingAndScanningExample.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PrintingAndScanningExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Fields

        private readonly PrintHelper _printHelper;
        private readonly ScannerHelper _scannerHelper = new ScannerHelper();
        
        private readonly NavigationHelper _navigationHelper;
        private readonly PicturesViewModel _viewModel;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            // Create the print helper.  Set the picture provider to be the view model's list of pictures, if the view model is defined when they are requested.
            _printHelper = new PrintHelper(() => _viewModel == null ? new PictureModel[]{} : _viewModel.Pictures);

            // Create the view model.
            _viewModel = new PicturesViewModel(_printHelper);

            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
            _navigationHelper.SaveState += HandleNavigationHelperSaveState;
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


        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public PicturesViewModel ViewModel
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
            _printHelper.ConfigurePrinting();
        }

        /// <summary>
        /// Invoked immediately after the Page is unloaded and is no longer the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the navigation that has unloaded the current Page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _printHelper.DetachPrinting();
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void HandleScanButtonClicked(Object sender, RoutedEventArgs args)
        {
            var flyout = (Flyout)FlyoutBase.GetAttachedFlyout((FrameworkElement)sender);
            var flyoutContent = (FrameworkElement)flyout.Content;
            var scanningControlViewModel = new ScanningControlViewModel(_scannerHelper);
            scanningControlViewModel.GetScanners();

            // Subscribe to receive scanned pics and then close the dialog
            scanningControlViewModel.ScanCompleted += async (o, e) =>
            {
                await _viewModel.AddPicturesFromFiles(e.ScannedFiles);
                flyout.Hide();
            };

            scanningControlViewModel.ScanProgressChanged += (o, e) =>
            {
                Debug.WriteLine("Scan Progress: {0}", e.ScanProgress);
            };
            
            flyoutContent.DataContext = scanningControlViewModel;
            flyout.ShowAt((FrameworkElement)sender);
        }
    }
}
