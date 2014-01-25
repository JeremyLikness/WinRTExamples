namespace AccessibilityExample
{
    using System;

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Input;

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
                    NameError.Visibility = Visibility.Collapsed;
                    AgeError.Visibility = Visibility.Collapsed;
                    AccessibleNameError.Visibility = Visibility.Collapsed;
                    AccessibleAgeError.Visibility = Visibility.Collapsed;
                };
        }

        private void ChangeThemeOnClick(object sender, RoutedEventArgs e)
        {
            viewModel.ToggleTheme();
        }

        private void NonAccessibleResetOnClick(object sender, RoutedEventArgs e)
        {
            NameBox.Text = string.Empty;
            AgeBox.Text = string.Empty;
            NameError.Visibility = Visibility.Collapsed;
            AgeError.Visibility = Visibility.Collapsed;
        }

        private async void NonAccessibleSubmitOnClick(object sender, RoutedEventArgs e)
        {
            var error = false;
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                NameError.Visibility = Visibility.Visible;
                error = true;
            }
            else
            {
                NameError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(AgeBox.Text))
            {
                AgeError.Visibility = Visibility.Visible;
                error = true;
            }
            else
            {
                int age;
                if (!int.TryParse(AgeBox.Text, out age))
                {
                    AgeError.Visibility = Visibility.Visible;
                    error = true;
                }
                else
                {
                    AgeError.Visibility = Visibility.Collapsed;
                }
            }

            if (error)
            {
                return;
            }
            var dialog = new MessageDialog("Looks good!");
            await dialog.ShowAsync();
        }

        private void AccessibleResetOnClick(object sender, RoutedEventArgs e)
        {
            AccessibleNameBox.Text = string.Empty;
            AccessibleAgeBox.Text = string.Empty;
            AccessibleNameError.Visibility = Visibility.Collapsed;
            AccessibleAgeError.Visibility = Visibility.Collapsed;
        }

        private async void AccessibleSubmitOnClick(object sender, RoutedEventArgs e)
        {
            var error = false;
            if (string.IsNullOrWhiteSpace(AccessibleNameBox.Text))
            {
                AccessibleNameError.Visibility = Visibility.Visible;
                error = true;
            }
            else
            {
                AccessibleNameError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(AgeBox.Text))
            {
                AccessibleAgeError.Visibility = Visibility.Visible;
                error = true;
            }
            else
            {
                int age;
                if (!int.TryParse(AccessibleAgeBox.Text, out age))
                {
                    AccessibleAgeError.Visibility = Visibility.Visible;
                    error = true;
                }
                else
                {
                    AccessibleAgeError.Visibility = Visibility.Collapsed;
                }
            }

            if (error)
            {
                return;
            }
            var dialog = new MessageDialog("Looks good!");
            await dialog.ShowAsync();
        }

        private async void ImageOnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("Thanks for checking out these samples.");
            await messageDialog.ShowAsync();
        }

        private async void AccessibleImageButtonOnClick(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("Thanks for checking out these accessible samples.");
            await messageDialog.ShowAsync();
        }
    }
}
