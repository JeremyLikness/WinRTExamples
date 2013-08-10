namespace VisualStateExample
{
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private string layout = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();
            //this.SizeChanged += this.MainPageSizeChanged;
            //this.Loaded += this.MainPageLoaded;
            //this.LayoutUpdated += this.MainPageLayoutUpdated;
        }

        void MainPageLayoutUpdated(object sender, object e)
        {
            this.SetLayout();
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            this.SetLayout();
        }

        void MainPageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.SetLayout();
        }

        void SetLayout()
        {
            var orientation = ApplicationView.GetForCurrentView().Orientation;
            string newMode;

            if (orientation == ApplicationViewOrientation.Landscape)
            {
                newMode = ApplicationView.GetForCurrentView().IsFullScreen ? "FullScreenLandscape" : "Filled";
            }
            else
            {
                newMode = Window.Current.Bounds.Width <= 500 ? "Snapped" : "FullScreenPortrait";
            }

            if (newMode == this.layout)
            {
                return;
            }

            VisualStateManager.GoToState(this, newMode, true);
            this.layout = newMode;
        }
    }
}
