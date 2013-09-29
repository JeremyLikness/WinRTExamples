using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Toaster.Data;

namespace Toaster
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        public ViewModel ViewModel
        {
            get
            {
                return (ViewModel) Resources["ViewModel"];
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null)
            {
                return;
            }

            var toastType = e.Parameter.ToString();

            var toast = this.ViewModel.Toasts.FirstOrDefault(t => t.Toast.TemplateType == toastType);

            if (toast != null)
            {
                this.ViewModel.SelectedItem = toast;
            }
        }

        void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.ViewModel.Executed = async (result, message) =>
            {
                if (result)
                {
                    var msg = string.IsNullOrWhiteSpace(message) ? "The toast was scheduled." : message;
                    var successDialog = new MessageDialog(msg);
                    await successDialog.ShowAsync();
                }
                else
                {
                    var errorDialog = new MessageDialog(message, "An error occurred.");
                    await errorDialog.ShowAsync();
                }
            };
        }
    }
}
