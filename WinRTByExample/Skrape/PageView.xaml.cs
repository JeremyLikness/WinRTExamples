// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageView.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   View the details of the scraped page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape
{
    using System;
    using System.Threading.Tasks;

    using Skrape.Data;

    using Windows.Networking.Connectivity;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// View the details of the scraped page
    /// </summary>
    public sealed partial class PageView
    {
        /// <summary>
        /// The margin app bar open.
        /// </summary>
        private static readonly Thickness MarginAppBarOpen = new Thickness(10, 10, 10, 100);

        /// <summary>
        /// The margin app bar open.
        /// </summary>
        private static readonly Thickness MarginAppBarClosed = new Thickness(10, 10, 10, 10);

        /// <summary>
        /// Initializes a new instance of the <see cref="PageView"/> class.
        /// </summary>
        public PageView()
        {
            this.InitializeComponent();

            if (this.BottomAppBar != null)
            {
                this.BottomAppBar.Opened += (o, e) =>
                    {
                        WebControl.Margin = MarginAppBarOpen;
                    };

                this.BottomAppBar.Closed += (o, e) =>
                    {
                        WebControl.Margin = MarginAppBarClosed;
                    };
            }

            WebControl.LoadCompleted += this.WebControlLoadCompleted;
            
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.DataContext = CurrentViewModel.DataManager.CurrentPage;
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
            CurrentViewModel.RefreshCallback = this.Refresh;
            var page = CurrentViewModel.DataManager.CurrentPage = e.Parameter as SkrapedPage;

            if (page != null && page.Deleted)
            {
                VisualStateManager.GoToState(this, "DeletedState", false);
                return;
            }

            ToggleHtml.IsEnabled = false;
            
            VisualStateManager.GoToState(this, "LoadingState", false);
            VisualStateManager.GoToState(this, "HtmlState", false);
            
            if (page == null)
            {
                return;
            }

            if (NetworkInformation.GetInternetConnectionProfile() != null)
            {
                if (!page.Loaded)
                {
                    await CurrentViewModel.DataManager.Scraper.GetHtmlForWebPage(page);
                    page.Loaded = true;
                    await CurrentViewModel.DataManager.Manager.SavePage(page);
                }

                this.WebControl.Navigate(page.Url);
            }
            else
            {
                this.WebControl.NavigateToString(page.Html);
            }
            
            VisualStateManager.GoToState(this, "WebViewLoadingState", false);
        }
        
        /// <summary>
        /// The on navigated from.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CurrentViewModel.DataManager.CurrentImage = null;
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// The refresh.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Refresh()
        {
            ToggleHtml.IsEnabled = false;
            var page = CurrentViewModel.DataManager.CurrentPage;

            if (page == null || NetworkInformation.GetInternetConnectionProfile() == null)
            {
                return;
            }

            VisualStateManager.GoToState(this, "LoadingState", false);
            VisualStateManager.GoToState(this, "HtmlState", false);

            await CurrentViewModel.DataManager.Scraper.GetHtmlForWebPage(page);
            page.Loaded = true;
            await CurrentViewModel.DataManager.Manager.SavePage(page);

            if (!ToggleHtml.IsEnabled)
            {
                VisualStateManager.GoToState(this, "WebViewLoadingState", false);
            }
        }

        /// <summary>
        /// The web control load completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WebControlLoadCompleted(object sender, NavigationEventArgs e)
        {
            VisualStateManager.GoToState(this, "LoadedState", false);
            ToggleHtml.IsEnabled = true;
        }

        /// <summary>
        /// The back button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        /// <summary>
        /// The toggle switch.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            var toggleState = ((ToggleSwitch)sender).IsOn;
            VisualStateManager.GoToState(this, toggleState ? "TextState" : "HtmlState", false);
        }

        /// <summary>
        /// The flip view on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlipViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var uri = ((FlipView)sender).SelectedItem as Uri;
            if (uri != null)
            {
                CurrentViewModel.DataManager.CurrentImage = uri;
            }
        }
    }
}
