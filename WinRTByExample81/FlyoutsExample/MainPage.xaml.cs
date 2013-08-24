using Windows.UI.Xaml;

namespace FlyoutsExample
{
    using System;

    using Windows.UI.ApplicationSettings;
    using Windows.UI.Popups;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private readonly string[] borderStates = { "DefaultBorder", "NoBorder" };

        private int borderIndex;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += this.MainPageLoaded;
            SettingsPane.GetForCurrentView().CommandsRequested += MainPageCommandsRequested;
        }

        static void MainPageCommandsRequested(
            SettingsPane sender, 
            SettingsPaneCommandsRequestedEventArgs args)
        {
            var setting = new SettingsCommand("LoremIpsumSettings", "Lorem Ipsum", handler =>
            {
                var flyout = new FlyoutSettings();
                flyout.Show();
            });

            args.Request.ApplicationCommands.Add(setting);
        }

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            this.borderIndex = 0;
            VisualStateManager.GoToState(this, this.borderStates[this.borderIndex], false);
            VisualStateManager.GoToState(this, "Default", false);
        }   

        private void MenuFlyoutItemRed_OnClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Default", true);
        }

        private void MenuFlyoutItemYellow_OnClick(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Yellow", true);
        }

        private void MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.borderIndex = (this.borderIndex + 1) % 2;
            VisualStateManager.GoToState(this, this.borderStates[this.borderIndex], true);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog(ValueTextBox.Text, "You said:");
            await dialog.ShowAsync();
            ValueTextBox.Text = string.Empty;
        }
    }
}
