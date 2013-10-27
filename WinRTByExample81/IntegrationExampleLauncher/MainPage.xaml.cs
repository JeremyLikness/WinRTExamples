using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IntegrationExampleLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IStorageFile _selectedLauncherFile;

        private Contact _selectedContact;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that can be examined by overriding code. The event data is representative of the pending navigation that will load the current Page. Usually the most relevant property to examine is Parameter.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateFileLauncherControls();
            UpdateProtocolLauncherControls();
            base.OnNavigatedTo(e);
        }

        #region File Activation

        /// <summary>
        /// Handles the click event for the button that allows selection of a file to be used for file activation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void HandleFileActivationFileSelectionClick(Object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker
                                 {
                                     FileTypeFilter = { ".wrtbe" },
                                 };
            _selectedLauncherFile = await fileOpenPicker.PickSingleFileAsync();
            UpdateFileLauncherControls();
        }

        /// <summary>
        /// Updates the controls to be used for launching file activation.
        /// </summary>
        private void UpdateFileLauncherControls()
        {
            // Update the contents of the FileNameTextBox
            FileNameTextBox.Text = _selectedLauncherFile == null ? "(No File Selected)" : _selectedLauncherFile.Name;

            // Enable or disable the LaunchFileButton
            LaunchFileActivationButton.IsEnabled = _selectedLauncherFile != null;
        }

        /// <summary>
        /// Handles the click event for the launch file activation button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void HandleLaunchFileActivationClick(Object sender, RoutedEventArgs e)
        {
            // Determine the selected view option from the UI
            var radioButtons = FileViewPreferences.Children.OfType<RadioButton>();
            var selectedLauncherViewPreference = GetSelectedViewSizePreference(radioButtons);

            // Determine whether or not to show the application picker UI
            var displayApplicationPicker = DisplayFileApplicationPickerCheckBox.IsChecked.GetValueOrDefault();

            // Compose the options and make the call
            var launcherOptions = new LauncherOptions
            {
                DesiredRemainingView = selectedLauncherViewPreference,
                DisplayApplicationPicker = displayApplicationPicker,
            };
            await Launcher.LaunchFileAsync(_selectedLauncherFile, 
                launcherOptions);
        }

        #endregion


        #region Protocol Activation

        /// <summary>
        /// Handles the click event for the button that allows selection of a contact to be used for procotol activation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void HandleProtocolActivationContactSelection(Object sender, RoutedEventArgs e)
        {
            var contactPicker = new ContactPicker();
            _selectedContact = await contactPicker.PickContactAsync();
            UpdateProtocolLauncherControls();
        }

        /// <summary>
        /// Updates the controls to be used for launching protocol activation.
        /// </summary>
        private void UpdateProtocolLauncherControls()
        {
            // Update the contents of the FileNameTextBox
            ContactNameTextBox.Text = _selectedContact == null ? "(No Contact Selected)" : _selectedContact.DisplayName;

            // Enable or disable the LaunchFileButton
            LaunchProtocolActivationButton.IsEnabled = _selectedContact != null;
        }

        /// <summary>
        /// Handles the click event for the launch protocol activation button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void HandleLaunchProtocolActivationClick(Object sender, RoutedEventArgs e)
        {
            // Determine the selected view option from the UI
            var radioButtons = ProtocolViewPreferences.Children.OfType<RadioButton>();
            var selectedLauncherViewPreference = GetSelectedViewSizePreference(radioButtons);

            // Determine whether or not to show the application picker UI
            var displayApplicationPicker = DisplayProtocolApplicationPickerCheckBox.IsChecked.GetValueOrDefault();

            // Build the URI to call
            var uriBuilder = new UriBuilder
            {
                Scheme = "wrtbe-integration",
                Host = _selectedContact.Id,
            };

            // Compose the options and make the call
            var launcherOptions = new LauncherOptions
            {
                DesiredRemainingView = selectedLauncherViewPreference,
                DisplayApplicationPicker = displayApplicationPicker,
            };
            await Launcher.LaunchUriAsync(uriBuilder.Uri, launcherOptions);
        } 
        #endregion

        private ViewSizePreference GetSelectedViewSizePreference(IEnumerable<RadioButton> preferenceRadioButtons)
        {
            if (preferenceRadioButtons == null) throw new ArgumentNullException("preferenceRadioButtons");

            var selectedRadioButton = preferenceRadioButtons.FirstOrDefault(x => x.IsChecked.GetValueOrDefault());
            var senderButtonTag = selectedRadioButton.Tag.ToString();

            ViewSizePreference selectedLauncherViewPreference;
            Enum.TryParse(senderButtonTag, true, out selectedLauncherViewPreference);
            return selectedLauncherViewPreference;
        }
    }
}
