namespace SplitWordsCSharp
{
    using MyLibrary;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// The splitter
        /// </summary>
        private readonly WordSplitter splitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" class/>
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.splitter = new WordSplitter();
        }

        /// <summary>
        /// Invoked when the button is clicked
        /// </summary>
        /// <param name="sender">The sender (in this case, the button)</param>
        /// <param name="e">Event arguments associated with the click event</param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            SplitText.ItemsSource = this.splitter.Split(TextToSplit.Text);
        }
    }
}
