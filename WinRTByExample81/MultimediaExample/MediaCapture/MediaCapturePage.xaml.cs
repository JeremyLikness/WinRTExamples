using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MultimediaExample.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MultimediaExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MediaCapturePage : Page
    {
        #region Fields

        private readonly NavigationHelper _navigationHelper;
        private readonly MediaCaptureViewModel _viewModel; 

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCapturePage"/> class.
        /// </summary>
        public MediaCapturePage()
        {
            var mediaCaptureHelper = new MediaCaptureHelper();
            _viewModel = new MediaCaptureViewModel(mediaCaptureHelper);
            _viewModel.UpdateCaptureDevices();

            mediaCaptureHelper.CaptureSettingsReset  += (sender, args) =>
            {
                mediaCaptureHelper.StartCapturePreview(CaptureElementItem);
                mediaCaptureHelper.SetPreviewMirroring(_viewModel.IsPreviewMirrored);
            };
            
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        public MediaCaptureViewModel ViewModel
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
        private void navigationHelper_LoadState(Object sender, LoadStateEventArgs e)
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
        private void navigationHelper_SaveState(Object sender, SaveStateEventArgs e)
        {
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
            //_captureHelper.Capture(PreviewSurface, CameraCaptureUIMode.PhotoOrVideo);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }

    public class DesignMediaCaptureViewModel : MediaCaptureViewModel
    {
        public DesignMediaCaptureViewModel()
            : base(new MediaCaptureHelper())
        {
        }
    }
}
