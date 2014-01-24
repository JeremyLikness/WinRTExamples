namespace AccessibilityExample
{
    using Windows.UI.Xaml;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private ViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += (o, e) =>
                {
                    this.DataContext = viewModel = viewModel ?? new ViewModel();                    
                };
        }

        private void ChangeThemeOnClick(object sender, RoutedEventArgs e)
        {
            viewModel.ToggleTheme();
        }
    }
}
