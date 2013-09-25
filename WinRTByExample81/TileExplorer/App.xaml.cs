using Windows.Storage;
using Windows.UI.Notifications;
using TileExplorer.DataModel;
using WinRTByExample.NotificationHelper.Tiles;

namespace TileExplorer
{
    using Common;

    using System;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private const string Notifications = "Notifications";

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static DataSource CurrentDataSource
        {
            get
            {
                return ((App) Current).DataSource;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether notifications on.
        /// </summary>
        public static bool NotificationsOn
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey(Notifications)
                    && (bool)ApplicationData.Current.LocalSettings.Values[Notifications];
            }

            set
            {
                ApplicationData.Current.LocalSettings.Values[Notifications] = value;
            }
        }

        /// <summary>
        /// Gets the main data source
        /// </summary>
        public DataSource DataSource { get; private set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            DataSource = new DataSource();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
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
                var navigationType = typeof (GroupedItemsPage);
                var navigationArgs = "AllGroups";

                if (e.Arguments.StartsWith("Id="))
                {
                    navigationType = typeof (ItemDetailPage);
                    navigationArgs = e.Arguments.Split('=')[1];
                }

                if (!rootFrame.Navigate(navigationType, navigationArgs))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("firstRun"))
            {
                return;
            }

            // set some default tiles 
            TileTemplateType.TileWide310x150Text03.GetTile()
                                .AddText("Tile Explorer")
                                .WithNoBranding()
                                .WithTile(TileTemplateType.TileSquare150x150Text03.GetTile()
                                    .AddText("Tile Explorer")
                                    .AddText("A WinRT Example")
                                    .AddText("by Jeremy Likness"))                               
                                .Set();

            ApplicationData.Current.LocalSettings.Values["firstRun"] = true;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
