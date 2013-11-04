using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Live;

namespace LiveConnectExample
{
    public sealed partial class AccountSettingsFlyout : SettingsFlyout
    {
        private readonly LiveConnectWrapper _liveConnectWrapper;

        public AccountSettingsFlyout()
        {
            InitializeComponent();
            
            _liveConnectWrapper = ((App)Application.Current).LiveConnectWrapper;
            
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                UpdateLoginStatus(false);
            }
        }

        private async void UpdateLoginStatus(Boolean loginIfDisconnected)
        {
            var connectionResult = await _liveConnectWrapper.UpdateConnectionAsync(loginIfDisconnected);
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

        private void SignInClick(Object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateLoginStatus(true);
            }
            catch (InvalidOperationException ex)
            {
                var messageDialog = new MessageDialog("An error occurred during login - " + ex.Message, "Login Error");
                messageDialog.ShowAsync();
            }
        }

        private async void SignOutClick(Object sender, RoutedEventArgs e)
        {
            var connectionResult = await _liveConnectWrapper.DisconnectAsync();
            await UpdateControls(connectionResult.SessionStatus, connectionResult.CanLogout);
        }
    }
}
