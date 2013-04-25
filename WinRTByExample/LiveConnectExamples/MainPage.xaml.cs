// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Main page to demonstrate single sign-on and obtaining profile information from the Live Connect SDK
// </summary>
// <remarks>
//   To be accepted into the Windows Store this app would need to provide "Account" and "Privacy Statement" links from the 
//   settings flyout 
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

namespace LiveConnectExamples
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Live;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media.Imaging;
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
        }

        /// <summary>
        /// Gets or sets the session (stored at the application level)
        /// </summary>
        private static LiveConnectSession Session
        {
            get
            {
                return ((App)Application.Current).Session;
            }

            set
            {
                ((App)Application.Current).Session = value;
            }
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Session == null)
            {
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    var client = new LiveAuthClient();
                    var result = await client.LoginAsync(new List<string> { "wl.signin", "wl.basic", "wl.birthday" });
                    if (result.Status == LiveConnectSessionStatus.Connected)
                    {
                        Session = result.Session;
                    }
                }
            }

            if (Session == null)
            {
                return;
            }

            try
            {
                var connect = new LiveConnectClient(Session);
                var result = await connect.GetAsync("me");
                dynamic profile = result.Result;
                this.UserName.Text = profile.name;

                if (profile.birth_year != null) 
                {
                    var age = DateTime.Now.Year - (int)profile.birth_year;
                    if (age > 0)
                    {
                        UserAge.Text = age.ToString();
                    }
                }

                var imageResult = await connect.GetAsync("me/picture");
                dynamic image = imageResult.Result;
                this.ProfilePicture.Source = 
                    new BitmapImage(
                        new Uri(image.location, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                this.UserName.Text = ex.Message;
            }
        }
    }
}
