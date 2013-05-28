// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Main page to set and send a toast
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Toaster
{
    using System;
    using System.Linq;

    using Toaster.Data;

    using Windows.UI.Popups;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        private ViewModel ViewModel
        {
            get
            {
                return (ViewModel)Resources["ViewModel"];
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

        /// <summary>
        /// The main page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainPageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.ViewModel.Executed = async (result, message) =>
            {
                if (result)
                {
                    var msg = string.IsNullOrWhiteSpace(message) ? "The toast was scheduled." : message;
                    var successdialog = new MessageDialog(msg);
                    await successdialog.ShowAsync();
                }
                else
                {
                    var errorDialog = new MessageDialog(message, "An Error Occurred.");
                    await errorDialog.ShowAsync();
                }
            };
        }
    }
}
