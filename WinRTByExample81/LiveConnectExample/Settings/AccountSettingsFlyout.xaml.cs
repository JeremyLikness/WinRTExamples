using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Live;

namespace LiveConnectExample
{
    public sealed partial class AccountSettingsFlyout : SettingsFlyout
    {
        private readonly LiveConnectWrapper _liveConnectWrapper;
        private readonly IDialogService _dialogService = new DialogService();

        public AccountSettingsFlyout()
        {
            InitializeComponent();
            
            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;
            
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                UpdateLoginStatus();
            }
        }

        private async void UpdateLoginStatus()
        {
            var connectionResult = await _liveConnectWrapper.UpdateConnectionAsync();
            await UpdateControls(connectionResult.SessionStatus, connectionResult.CanLogout);
        }

        private async Task UpdateControls(LiveConnectSessionStatus sessionStatus, Boolean canLogout)
        {
            SignInButton.Visibility = sessionStatus != LiveConnectSessionStatus.Connected
                ? Visibility.Visible
                : Visibility.Collapsed;
            SignOutButton.Visibility = sessionStatus == LiveConnectSessionStatus.Connected && canLogout
                ? Visibility.Visible
                : Visibility.Collapsed;
            SignOutUnavailableText.Visibility = sessionStatus == LiveConnectSessionStatus.Connected && !canLogout
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (sessionStatus == LiveConnectSessionStatus.Connected)
            {
                await UpdateUserName();
            }
            else
            {
                SignInText.Text = "Not currently signed in...";
            }
        }

        private async Task UpdateUserName()
        {
            var profile = await _liveConnectWrapper.GetMyProfileAsync();
            SignInText.Text = String.Format("Logged in as {0}", profile.name);
        }

        private async void SignInClick(Object sender, RoutedEventArgs e)
        {
            try
            {
                // Show the visual signin
                var connectionResult = await _liveConnectWrapper.ShowLogin();
                await UpdateControls(connectionResult.SessionStatus, connectionResult.CanLogout);
            }
            catch (InvalidOperationException ex)
            {
                _dialogService.ShowError("An error occurred during login - " + ex.Message);
            }
        }

        private async void SignOutClick(Object sender, RoutedEventArgs e)
        {
            var connectionResult = await _liveConnectWrapper.DisconnectAsync();
            await UpdateControls(connectionResult.SessionStatus, connectionResult.CanLogout);
        }
    }
}
