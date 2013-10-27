using System;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ShareTargetExample.Common;

namespace ShareTargetExample
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper _navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        public MainPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += OnNavigationHelperLoadState;
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
        private void OnNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            //this.DefaultViewModel["Groups"] = sampleDataGroups;
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

        private async void OnClickLaunchShareSourceExampleApp(Object sender, RoutedEventArgs e)
        {
            // Because the share source app has a protocol activation set for it in order to showcase the app links,
            // we can take advantage of that and use the protocol activation to launch it directly from this target app.
            //
            // We use the options to tell Windows that we'd prefer that the Source Example app take the full screen, rather 
            // than share with this Target Example landing screen.
            var options = new Windows.System.LauncherOptions
                          {
                              DesiredRemainingView = ViewSizePreference.UseNone,
                              DisplayApplicationPicker = false,
                          };

            // Fancy stuff - if not installed, show the Windows UI dialog that talks about getting the app from the store directly below the button.
            // TO do this, the hard part is actually finding and supplying the button's rect
            var button = (Button)sender;
            var rect = button.GetBoundingRect();
            options.UI.PreferredPlacement = Placement.Below;
            options.UI.SelectionRect = rect;

            await Windows.System.Launcher.LaunchUriAsync(new Uri("wrtbe-share:applaunch"), options);
        }
    }

    public static partial class Extensions
    {
        public static Rect GetBoundingRect(this FrameworkElement element)
        {
            var buttonTransform = element.TransformToVisual(null);
            var transformPoint = buttonTransform.TransformPoint(new Point());
            var rect = new Rect
            {
                X = transformPoint.X,
                Y = transformPoint.Y,
                Width = element.ActualWidth,
                Height = element.ActualHeight,
            };
            return rect;
        }
    }
}