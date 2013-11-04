using System;
using System.Linq;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Microsoft.WindowsAzure.MobileServices;
using MobileServicesExample.Common;

namespace MobileServicesExample
{
    public class CredentialsViewModel
    {
        private RelayCommand _loginCommand;
        private RelayCommand _logoutCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                       (_loginCommand = new RelayCommand(
                           GetOrLoginUser,
                           CanLoginUser
                           ));
            }
        }

        public RelayCommand LogoutCommand
        {
            get
            {
                return _logoutCommand ??
                       (_logoutCommand = new RelayCommand(
                           LogoutUser,
                           CanLogoutUser
                           ));
            }
        }

        /// <summary>
        /// Gets a logged in user if one exists, or prompts for login.
        /// </summary>
        /// <returns>The logged in <see cref="MobileServiceUser"/> or <c>null</c> otherwise.</returns>
        /// <remarks>
        /// </remarks>
        private async void GetOrLoginUser()
        {
            // Check to see if there's a stored user in the PasswordVault
            var user = GetStoredUser();
            if (user == null)
            {
                String errorMessage;
                try
                {
                    user = await App.WinRTByExampleBookClient
                        .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                    SetStoredUser(user);
                    CompleteSuccessfulLogin();
                    return;
                }
                catch (InvalidOperationException e)
                {
                    // This exception is thrown if the user cancels the login.
                    // TODO - React to cancellation.
                    errorMessage = e.Message;
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
                var message = new MessageDialog("Login failed: " + errorMessage, "Login failed");
                await message.ShowAsync();
            }
            else
            {
                // A user was returned...set it as the client's current user
                App.WinRTByExampleBookClient.CurrentUser = user;
            }

            CompleteSuccessfulLogin();
            // TODO - For use when integrating with LiveId - http://www.windowsazure.com/en-us/develop/mobile/tutorials/single-sign-on-windows-8-dotnet/
            // MobileServices.Client.LoginWithMicrosoftAccountAsync();
        }

        private void CompleteSuccessfulLogin()
        {
            LoginCommand.RaiseCanExecuteChanged();
            LogoutCommand.RaiseCanExecuteChanged();

            // Upload the Notification Channel
            // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409
            WinRTByExampleBookPush.UploadChannel();
        }

        private Boolean CanLoginUser()
        {
            return App.WinRTByExampleBookClient.CurrentUser == null;
        }

        private void LogoutUser()
        {
            // Clear the credential vault
            ClearStoredCredentials();

            // Clear the credentials in the Mobile Service Client
            App.WinRTByExampleBookClient.Logout();
            LoginCommand.RaiseCanExecuteChanged(); 
            LogoutCommand.RaiseCanExecuteChanged();
        }

        private Boolean CanLogoutUser()
        {
            return !CanLoginUser();
        }

        private static MobileServiceUser GetStoredUser()
        {
            // Locate a stored credential for the current mobile service
            var vault = new PasswordVault();
            PasswordCredential credential = null;
            try
            {
                credential =
                    vault.FindAllByResource(App.WinRTByExampleBookClient.ApplicationUri.DnsSafeHost).FirstOrDefault();
            }
            catch (Exception)
            {
                // The PasswordVault API throws a general Exception if a resource store is not found.
            }
            if (credential == null) return null;

            // Get the saved user
            var user = new MobileServiceUser(credential.UserName);
            // Fetch the password into memory
            credential.RetrievePassword();
            // Set the authentication token
            user.MobileServiceAuthenticationToken = credential.Password;
            return user;
        }

        private static void SetStoredUser(MobileServiceUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            var vault = new PasswordVault();
            ClearStoredCredentials(vault);

            var newCredential = new PasswordCredential(
                App.WinRTByExampleBookClient.ApplicationUri.DnsSafeHost,
                user.UserId,
                user.MobileServiceAuthenticationToken);
            vault.Add(newCredential);
        }

        private static void ClearStoredCredentials()
        {
            var vault = new PasswordVault();
            ClearStoredCredentials(vault);
        }

        private static void ClearStoredCredentials(PasswordVault vault)
        {
            if (vault == null) throw new ArgumentNullException("vault");
            try
            {
                var credentials = vault.FindAllByResource(App.WinRTByExampleBookClient.ApplicationUri.DnsSafeHost);
                if (credentials != null)
                {
                    foreach (var credential in credentials)
                    {
                        vault.Remove(credential);
                    }
                }
            }
            catch (Exception)
            {
                // The PasswordVault API throws a general Exception if a resource store is not found.
            }
        }
 
    }
}