// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Provides application-specific behavior to supplement the default Application class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TileExplorer
{
    using System;

    using TileExplorer.Common;
    using TileExplorer.DataModel;

    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Storage;
    using Windows.UI.Notifications;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using WinRTByExample.NotificationHelper.Tiles;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        /// The notifications.
        /// </summary>
        private const string Notifications = "Notifications";

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class. 
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Gets the current data source.
        /// </summary>
        public static DataSource CurrentDataSource
        {
            get
            {
                return ((App)Current).DataSource;
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
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            DataSource = new DataSource();            

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                var navigationType = typeof(GroupedItemsPage);
                var navigationArgs = "AllGroups";

                if (args.Arguments.StartsWith("Id="))
                {
                    navigationType = typeof(ItemDetailPage);
                    navigationArgs = args.Arguments.Split('=')[1];
                }

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
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
            TileTemplateType.TileWideText03.GetTile()
                                .AddText("Tile Explorer")
                                .WithNoBranding()
                                .WithTile(TileTemplateType.TileSquareText03.GetTile()
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
        private static async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
