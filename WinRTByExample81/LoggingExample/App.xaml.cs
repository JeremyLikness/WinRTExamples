namespace LoggingExample
{
    using Windows.Foundation.Diagnostics;
    using Windows.Storage;

    using LoggingExample.Common;
    using LoggingHelper;
    using System;
    using System.Diagnostics.Tracing;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private EventListener appListener, errorListener;

        private bool winRtLoggingEnabled = false;

        private LoggingLevel winRtLogLevel;

        private LoggingSession session;
        private LoggingChannel channel;

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (appListener == null)
            {
                appListener = new LogEventListener();
                appListener.EnableEvents(LogEventSource.Log, EventLevel.Verbose);
                errorListener = new LogEventListener("Errors");
                errorListener.EnableEvents(LogEventSource.Log, EventLevel.Error);
                LogEventSource.Log.Info("App initialized.");

                // winRT approach
                channel = new LoggingChannel("WinRTChannel");
                channel.LoggingEnabled += (o, args) =>
                    {
                        this.winRtLoggingEnabled = o.Enabled;
                        this.winRtLogLevel = o.Level;
                    };
                session = new LoggingSession("WinRTSession");
                session.AddLoggingChannel(channel);
            }

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

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        const string Message = "Restoring saved session state.";
                        LogEventSource.Log.Info(Message);
                        channel.LogMessage(Message, LoggingLevel.Information);
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException ex)
                    {
                        var message = string.Format("Error restoring saved session state: {0}", ex.Message);
                        LogEventSource.Log.Error(message);
                        channel.LogMessage(message, LoggingLevel.Error);
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
                rootFrame.Navigate(typeof(HubPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            const string Msg = "Window activated."; 
            LogEventSource.Log.Info(Msg);
            channel.LogMessage(Msg, LoggingLevel.Information);            
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        async void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            var message = string.Format("Failed to navigate to page: {0}", e.SourcePageType.FullName);
            LogEventSource.Log.Critical(message);
            channel.LogMessage(message, LoggingLevel.Critical);
            var file =
                await session.SaveToFileAsync(ApplicationData.Current.TemporaryFolder, "log_" + DateTime.Now.Ticks);
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            const string SuspendMsg = "Suspending app.";
            LogEventSource.Log.Info(SuspendMsg);
            channel.LogMessage(SuspendMsg, LoggingLevel.Information);
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
