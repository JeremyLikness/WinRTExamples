using System;
namespace GlobalizationExample
{
    using Windows.UI.Xaml.Navigation;
    using Windows.ApplicationModel.Resources;
    using Windows.Globalization.NumberFormatting;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        const double CurrencyValue = 123.45;
            
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var loader = ResourceLoader.GetForCurrentView();
            DynamicText.Text = loader.GetString("Dynamic");

            var currencyText = loader.GetString("Dollars");
            var currencyFormatter =
                new CurrencyFormatter(Windows.System.UserProfile.GlobalizationPreferences.Currencies[0]);
            Currency.Text = string.Format(currencyText, currencyFormatter.Format(CurrencyValue));

            var dateFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shortdate");
            var timeFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shorttime");

            Date.Text = dateFormatter.Format(DateTime.Now);
            Time.Text = timeFormatter.Format(DateTime.Now);
        }
    }
}
