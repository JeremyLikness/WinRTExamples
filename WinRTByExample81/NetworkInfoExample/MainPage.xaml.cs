namespace NetworkInfoExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => this.Connections.ScrollIntoView(this.Connections.SelectedItem);
        }
    }
}
