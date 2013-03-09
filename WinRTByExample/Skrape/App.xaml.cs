// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Provides application-specific behavior to supplement the default Application class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape
{
    using System;

    using Skrape.Data;

    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class. 
        /// </summary>
        public App()
        {
            this.InitializeComponent();           
        }

        /// <summary>
        /// Gets the current resource.
        /// </summary>
        public GlobalViewModel CurrentViewModel
        {
            get
            {
                return (GlobalViewModel)Resources["GlobalResources"];
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            var dataManager = new SkrapeDataManager();
            var webScraper = new WebScraper();
            var pageManager = new PageAndGroupManager();
            dataManager.Scraper = webScraper;
            dataManager.Manager = pageManager;
            
            this.CurrentViewModel.DataManager = dataManager;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            this.CurrentViewModel.GoHome = () => rootFrame.Navigate(typeof(MainPage));

            // Ensure the current window is active
            Window.Current.Activate();
        }        
    }
}
