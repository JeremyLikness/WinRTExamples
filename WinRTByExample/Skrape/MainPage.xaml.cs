// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape
{
    using System;

    using Skrape.Data;

    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
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
        /// Gets the current resource.
        /// </summary>
        private static GlobalViewModel CurrentViewModel
        {
            get
            {
                return ((App)Application.Current).CurrentViewModel;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            VisualStateManager.GoToState(this, "Loading", false);

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                await CurrentViewModel.DataManager.Initialize();
                CurrentViewModel.AddCallback = () =>
                    { AddPopup.IsOpen = true; };                    
            }

            CurrentViewModel.DataManager.CurrentPage = new SkrapedPage();
            CurrentViewModel.DataManager.CurrentGroup = new SkrapeGroup();

            VisualStateManager.GoToState(this, "Loaded", false);
        }

        /// <summary>
        /// The on item click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(PageView), e.ClickedItem as SkrapedPage);
        }

        /// <summary>
        /// The button base_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        /// <summary>
        /// The add button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            Uri uri;
            if (Uri.TryCreate(NewUrl.Text, UriKind.Absolute, out uri))
            {
                await CurrentViewModel.DataManager.AddUrl(uri);
                NewUrl.Text = string.Empty;
            }
            else
            {
                var dialog = new MessageDialog("The URL was invalid. Try using the format http(s)://something...");
                await dialog.ShowAsync();
            }
            
            AddPopup.IsOpen = false;
        }
    }
}
