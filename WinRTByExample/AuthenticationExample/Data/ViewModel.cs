// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace AuthenticationExample.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using AuthenticationExample.Auth;
    using AuthenticationExample.Common;
    using AuthenticationExample.Identity;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : BindableBase, ICommand 
    {
        /// <summary>
        /// The authentication types.
        /// </summary>
        private readonly List<AuthenticationType> authenticationTypes;

        /// <summary>
        /// The console.
        /// </summary>
        private readonly ObservableCollection<string> console = new ObservableCollection<string>();

        /// <summary>
        /// The credential storage
        /// </summary>
        private readonly ICredentialStorage credentialStorage;

        /// <summary>
        /// Selected authentication type
        /// </summary>
        private AuthenticationType selectedType;

        /// <summary>
        /// Authentication status
        /// </summary>
        private string authenticationStatus;

        /// <summary>
        /// Email from identity
        /// </summary>
        private string email;

        /// <summary>
        /// The selected console item
        /// </summary>
        private string consoleItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.credentialStorage = new AppCredentialStorage();
            var googleAuth = new GoogleAuthenticator(this.credentialStorage)
                                 {
                                     LogToConsole = this.Log
                                 };
            var facebookAuth = new FacebookAuthenticator(this.credentialStorage)
                                   {
                                       LogToConsole = this.Log
                                   };

            this.authenticationTypes =
                new List<AuthenticationType>
                {
                    new AuthenticationType { Name = "None" }, 
                    new AuthenticationType
                    {
                        Name = facebookAuth.Name, 
                        Auth = facebookAuth, 
                        Identity = new FacebookIdentity
                                       {
                                           LogToConsole = this.Log
                                       }
                    }, 
                    new AuthenticationType
                    {
                        Name = googleAuth.Name, 
                        Auth = googleAuth,
                        Identity = new GoogleIdentity
                                       {
                                           LogToConsole = this.Log
                                       }
                    }
                };

            this.selectedType = this.authenticationTypes[0];
            this.authenticationStatus = "Not Authenticated";
            this.email = "N/A";

            this.Log("Application initialized.");

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            this.Log("This is just a test message.");
            this.Log("To show the console when the display mode is set to design time enabled, and this is specifically a longer message to make it easier to test that text is wrapping appropriately.");
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Gets the authentication types.
        /// </summary>
        public IEnumerable<AuthenticationType> AuthenticationTypes
        {
            get
            {
                return this.authenticationTypes;
            }
        }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public AuthenticationType SelectedType
        {
            get
            {
                return this.selectedType;
            }
            
            set
            {
                this.selectedType = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the authentication status.
        /// </summary>
        public string AuthenticationStatus
        {
            get
            {
                return this.authenticationStatus;
            }

            set
            {
                this.authenticationStatus = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                this.email = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the console item.
        /// </summary>
        public string ConsoleItem
        {
            get
            {
                return this.consoleItem;
            }

            set
            {
                this.consoleItem = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the console.
        /// </summary>
        public ObservableCollection<string> Console
        {
            get
            {
                return this.console;
            }
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            if (parameter != null && parameter.ToString().Equals("signout"))
            {
                this.credentialStorage.Signout(this.selectedType.Name);
                this.Log(string.Format("Signed out of {0}", this.selectedType.Name));
                return;
            }

            Action asyncExecute = async () =>
                {
                    if (this.selectedType.Auth == null)
                    {
                        this.Email = "N/A";
                        this.AuthenticationStatus = "Not Authenticated";
                    }
                    else
                    {
                        this.Email = "N/A";
                        this.AuthenticationStatus = "Authenticating...";

                        try
                        {
                            var token = await this.selectedType.Auth.AuthenticateAsync(new[] { "email" });
                            this.AuthenticationStatus = string.Format("Authenticated with {0}.", this.selectedType.Name);
                            var emailAddress = await this.selectedType.Identity.GetEmail(token);
                            this.Email = string.Format("{0} email: {1}", this.selectedType.Name, emailAddress);
                        }
                        catch (Exception ex)
                        {
                            this.AuthenticationStatus = ex.Message;
                        }
                    }
                };
            
            asyncExecute();
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void Log(string message)
        {
            var msg = string.Format("{0} {1}", DateTime.Now, message);
            this.console.Add(msg);
            this.ConsoleItem = msg;
        }
    }
}