// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LayoutsExample
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

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
            this.Loaded += this.MainPageLoaded;
        }

        /// <summary>
        /// The main page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = LayoutRoot.Resources["ViewModel"] as ViewModel;
            
            if (viewModel == null)
            {
                return;
            }
            viewModel.GoToVisualState = state => VisualStateManager.GoToState(this, state, true);
            viewModel.GoToStretch =
                stretch =>
                {
                    var newStretch = (Stretch)Enum.Parse(typeof(Stretch), stretch);
                    this.ViewBoxInstance.SetValue(Viewbox.StretchProperty, newStretch);
                };
        }        
    }
}
