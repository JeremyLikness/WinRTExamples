using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using IntegrationExample.Common;
using IntegrationExample.Data;

namespace IntegrationExample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {

#if DEBUG
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                //DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            await HandleBasicActivation(args);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(Object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display a file open picker.
        /// </summary>
        /// <param name="e">Details about the activation request.</param>
        protected async override void OnFileOpenPickerActivated(FileOpenPickerActivatedEventArgs e)
        {
            // Create the user interface element to be displayed within the picker area
            var fileOpenPickerPage = new FileOpenPickerPage();

            // Activate the current Window
            Window.Current.Content = fileOpenPickerPage;
            Window.Current.Activate();

            // TODO - locate this better
            await LoadSampleDataAsync();

            // Initialize the window to display its UI
            fileOpenPickerPage.Initialize(e.FileOpenPickerUI);
        }
        
        /// <summary>
        /// Invoked when the application is activated to display a file open picker.
        /// </summary>
        /// <param name="e">Details about the activation request.</param>
        protected async override void OnFileSavePickerActivated(FileSavePickerActivatedEventArgs e)
        {
            // Create the user interface element to be displayed within the picker area
            var fileSavePickerPage = new FileSavePickerPage();

            // Activate the current Window
            Window.Current.Content = fileSavePickerPage;
            Window.Current.Activate();

            // TODO - locate this better
            await LoadSampleDataAsync();

            fileSavePickerPage.Initialize(e.FileSavePickerUI);
        }

        /// <summary>
        /// Invoked when the application is activated through file-open.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected async override void OnFileActivated
            (FileActivatedEventArgs args)
        {
            await HandleBasicActivation(args);

            if (args.Verb == "ScanWRTBEFiles")
            {
                var rootFolder = args.Files.OfType<StorageFolder>().FirstOrDefault();
                if (rootFolder != null)
                {
                    // Potentially recursively scan through all folders.
                    // For now just use files at the root.
                    var queryOptions = new QueryOptions(
                        CommonFileQuery.DefaultQuery, 
                        new[] { ".wrtbe" });
                    var query = rootFolder.CreateFileQueryWithOptions(queryOptions);
                    var contactFiles = await query.GetFilesAsync();
                    foreach (var contactFile in contactFiles)
                    {
                        await SampleData.ProcessActivationFile(contactFile);
                    }
                }
            }
            else
            {
                foreach (var storageFile in args.Files.OfType<IStorageFile>())
                {
                    // For multiple registrations, examine the file type
                    await SampleData.ProcessActivationFile(storageFile);
                }
            }
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ContactPicker)
            {
                // Create the user interface element to be displayed 
                // within the picker area
                var contactPickerPage = new ContactPickerPage();

                // Activate the current Window
                Window.Current.Content = contactPickerPage;
                Window.Current.Activate();

                // TODO - locate this better
                await LoadSampleDataAsync();
                
                var contactPickerArgs = (ContactPickerActivatedEventArgs)args;
                contactPickerPage.Initialize(contactPickerArgs.ContactPickerUI);
            }
            // Handle arrangements necessary if the app was launched via 
            // a protocol request
            else if (args.Kind == ActivationKind.Protocol)
            {
                // Extract the arguments and do basic activation
                var protocolArgs = (ProtocolActivatedEventArgs) args;
                await HandleBasicActivation(protocolArgs);

                var providedUri = protocolArgs.Uri;
                if (providedUri.Scheme == "wrtbe-integration")
                {
                    // If an Id is provided in the Uri, use it to try to locate 
                    // and navigate to the details page for a contact with the 
                    // matching Id
                    var itemId = providedUri.Host;
                    if (!String.IsNullOrWhiteSpace(itemId) &&
                        SampleData.GetItem(itemId) != null)
                    {
                        var rootFrame = (Frame) Window.Current.Content;
                        rootFrame.Navigate(typeof (ContactDetailPage), itemId);
                    }
                }
                
                    // Here you would do app-specific logic so that the user receives account picture selection UX.
                else if (providedUri.Scheme == "ms-accountpictureprovider")
                {
                    // The app was activated as an Account Picture Provider from PC Settings, Accounts, Your Account, Create an account picture.
                    // Normally, this point would be used to navigate to a page specific to setting and working with such a picture.
                    // In this app, the picture can simply be set by choosing an image attached to one of the tracked contacts.
                }
            }
            //else if (args.Kind == ActivationKind.Device)
            //{
            //    var deviceArgs = (DeviceActivatedEventArgs) args;
            //    var deviceInformationId = deviceArgs.DeviceInformationId;
            //    var verb = deviceArgs.Verb;
            //}
            else
            {
                base.OnActivated(args);
            }
        }

        private async Task HandleBasicActivation(IActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                // TODO - locate this better
                await LoadSampleDataAsync();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(GroupedItemsPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private SampleDataSource _sampleDataSource;
        public SampleDataSource SampleData
        {
            get { return _sampleDataSource ?? (_sampleDataSource = new SampleDataSource()); }
        }

        private Boolean _isSampleDataLoaded;
        private async Task LoadSampleDataAsync()
        {
            // TODO - get rid of this (yuck!)
            if (_isSampleDataLoaded) return;

            var dataUri = new Uri("ms-appx:///DataModel/SampleData.json");
            var sampleDataFile = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            await SampleData.ProcessActivationFile(sampleDataFile);
            _isSampleDataLoaded = true;
        }
    }

    public static partial class Extensions
    {
        public static SampleDataSource GetSampleData(this Application currentApplication)
        {
            return ((App) Application.Current).SampleData;
        }
    }
}
