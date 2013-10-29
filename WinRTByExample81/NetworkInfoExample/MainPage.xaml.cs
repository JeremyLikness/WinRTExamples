namespace NetworkInfoExample
{
    using NetworkInfoExample.Data;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += async (sender, args) =>
                {
                    var vm = new ViewModel();
                    await vm.Initialize();
                    RootGrid.DataContext = vm;
                    this.Connections.ScrollIntoView(this.Connections.SelectedItem);
                    this.Connections.SelectionChanged += 
                        (list, changeArgs) => this.Connections.ScrollIntoView(this.Connections.SelectedItem);
                };
        }
    }
}
