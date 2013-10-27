using System;
using System.Linq;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using IntegrationExample.Common;
using IntegrationExample.Data;

namespace IntegrationExample
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class ContactDetailPage : Page
    {
        private static readonly String[] ImageFileTypes = {".jpg", ".jpeg", ".png", ".bmp"};

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactDetailPage"/> class.
        /// </summary>
        public ContactDetailPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void HandleNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
            var contact = Application.Current.GetSampleData().GetItem((String)e.NavigationParameter);
            DefaultViewModel["Contact"] = contact;
            DefaultViewModel["Files"] = await contact.GetRelatedFiles();
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
            HandleUIEnabling();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void HandleAddFileClick(Object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            foreach (var item in ImageFileTypes)
            {
                fileOpenPicker.FileTypeFilter.Add(item);
            }
            
            //var selectedFile = await fileOpenPicker.PickSingleFileAsync();
            var selectedFiles = await fileOpenPicker.PickMultipleFilesAsync();
            foreach (var selectedFile in selectedFiles)
            {
                var currentContact = (Contact) DefaultViewModel["Contact"];
                var localFolder = ApplicationData.Current.LocalFolder;
                var selectedContactFolder =
                    await localFolder.CreateFolderAsync(currentContact.Id, CreationCollisionOption.OpenIfExists);
                var targetFile = await selectedContactFolder.CreateFileAsync(selectedFile.Name, CreationCollisionOption.GenerateUniqueName);
                await selectedFile.CopyAndReplaceAsync(targetFile);
            }
        }

        private void HandleSelectedItemChanged(Object sender, SelectionChangedEventArgs e)
        {
            HandleUIEnabling();
        }

        private void HandleUIEnabling()
        {
            var imageIsSelected = false;
            var selectedFileInfo = ItemGridView.SelectedItem as FileInfo;
            if (selectedFileInfo != null)
            {
                var selectedFile = selectedFileInfo.File;
                imageIsSelected =
                    ImageFileTypes.Any(x => x.Equals(selectedFile.FileType, StringComparison.OrdinalIgnoreCase));
            }
            SetAccountPicButton.IsEnabled = imageIsSelected;
        }

        private async void HandleSetAccountPicClick(Object sender, RoutedEventArgs e)
        {
            var selectedFileInfo = ItemGridView.SelectedItem as FileInfo;
            if (selectedFileInfo == null) return;

            var displayName = await UserInformation.GetDisplayNameAsync();
            var confirmMessage = String.Format("Are you sure you want to reset the account picture for {0}?",
                displayName);
            var yesCommand = new UICommand("Yes");
            var noCommand = new UICommand("No");
            var confirmDialog = new MessageDialog(confirmMessage, "Set Account Picture")
                                {
                                    Commands = { yesCommand, noCommand },
                                    DefaultCommandIndex = 1,
                                    CancelCommandIndex = 1
                                };
            if (await confirmDialog.ShowAsync() == noCommand) return;

            var selectedFile = selectedFileInfo.File;
            var setResult = await UserInformation.SetAccountPicturesAsync(null, selectedFile, null);
            var resultMessage = setResult == SetAccountPictureResult.Success
                ? "Successfully updated the account picture"
                : "The account picture could not be updated";

            var resultDialog = new MessageDialog(resultMessage, "Set Account Picture");
            await resultDialog.ShowAsync();
        }

        private void HandleContactImageTapped(Object sender, 
            TappedRoutedEventArgs e)
        {
            var senderElement = (FrameworkElement)sender;
            var itemRect = senderElement.GetElementRect();
            var contact = (Contact)DefaultViewModel["Contact"];
            ContactManager.ShowContactCard(contact, itemRect, Placement.Default);
        }

        private async void HandleShowWeekClick(Object sender, RoutedEventArgs e)
        {
            await AppointmentManager.ShowTimeFrameAsync(DateTimeOffset.Now, 
                TimeSpan.FromDays(7));
        }

        private async void HandleMakeAppointmentClick(Object sender, RoutedEventArgs e)
        {
            var senderElement = sender as FrameworkElement;
            var itemRect = senderElement.GetElementRect();
            var contact = (Contact)DefaultViewModel["Contact"];

            var appointment = new Appointment
            {
                Subject = String.Format("Sample appointment with {0}", contact.DisplayName),
                StartTime = DateTimeOffset.Now - TimeSpan.FromMinutes(DateTimeOffset.Now.Minute) + TimeSpan.FromHours(1),
                Duration = TimeSpan.FromHours(1),
                BusyStatus = AppointmentBusyStatus.Busy,
                AllDay = false,
                Details = "This is a sample appointment for the book WinRT by Example",
                Location = "Online",
            };
            //var appointmentId = 
                await AppointmentManager.ShowAddAppointmentAsync(
                appointment, itemRect, Placement.Default);
        }
    }

    public static partial class Extensions
    {
        public static Rect GetElementRect(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) throw new ArgumentNullException("frameworkElement");

            var transform = frameworkElement.TransformToVisual(null);
            var transformPoint = transform.TransformPoint(new Point());
            return new Rect(transformPoint, new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
        }
    }
}