// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The main page that lists the notes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SafeNotes
{
    using System;
    using System.Collections.Generic;

    using SafeNotes.Common;
    using SafeNotes.Data;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// The main page_ loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = ((App)Application.Current).CurrentViewModel;
            if (viewModel.IsInitialized)
            {
                return;
            }

            VisualStateManager.GoToState(this, "LoadingState", false);
            await viewModel.InitializeAsync();
            VisualStateManager.GoToState(this, "LoadedState", false);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            ((App)Application.Current).CurrentViewModel.GoHome = () =>
                {
                    if (this.Frame.CanGoBack)
                    {
                        this.Frame.GoBack();
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(MainPage));
                    }
                };
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
        }

        /// <summary>
        /// The main grid_ on item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MainGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var viewModel = ((App)Application.Current).CurrentViewModel;
            viewModel.CurrentNote = e.ClickedItem as SimpleNote;
            viewModel.SetEdit();
            this.Frame.Navigate(typeof(NotePage), viewModel.CurrentNote.Id);
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
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = ((App)Application.Current).CurrentViewModel;
            viewModel.SetNew();
            this.Frame.Navigate(typeof(NotePage), string.Empty);
        }
    }
}
