// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacebookAuthenticator.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The facebook authenticator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Auth
{
    using System;
    using System.Security;
    using System.Threading.Tasks;

    using AuthenticationExample.Data;

    using Windows.Security.Authentication.Web;

    /// <summary>
    /// The facebook authenticator.
    /// </summary>
    public class FacebookAuthenticator : IAuthHelper, ICanLog 
    {
        /// <summary>
        /// The face book id.
        /// </summary>
        private const string FacebookAppId = "{YourAppIdHere}"; // App ID from https://developers.facebook.com/apps 

        /// <summary>
        /// The facebook redirect uri.
        /// </summary>
        private const string FacebookRedirectUri = "https://www.facebook.com/connect/login_success.html";

        /// <summary>
        /// The face book authentication.
        /// </summary>
        private const string FaceBookAuth =
            "https://www.facebook.com/dialog/oauth?client_id={0}&redirect_uri={1}&scope={2}&response_type=token";

        /// <summary>
        /// The data storage.
        /// </summary>
        private readonly ICredentialStorage dataStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookAuthenticator"/> class.
        /// </summary>
        /// <param name="dataStorage">
        /// The data storage.
        /// </param>
        public FacebookAuthenticator(ICredentialStorage dataStorage)
        {
            this.dataStorage = dataStorage;
            this.LogToConsole = delegate { };
        }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "Facebook";
            }
        }

        /// <summary>
        /// Gets or sets the log to console.
        /// </summary>
        public Action<string> LogToConsole { get; set; }

        /// <summary>
        /// The authenticate async.
        /// </summary>
        /// <param name="claims">
        /// The claims.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<string> AuthenticateAsync(string[] claims)
        {
            string savedToken;
            if (this.dataStorage.TryRestore(this.Name, out savedToken))
            {
                this.LogToConsole(string.Format("Using saved token {0}", savedToken));
                return savedToken; 
            }

            var startUri = new Uri(string.Format(FaceBookAuth, FacebookAppId, FacebookRedirectUri, string.Join(",", claims)), UriKind.Absolute);
            var endUri = new Uri(FacebookRedirectUri, UriKind.Absolute);

            this.LogToConsole(string.Format("Attemping to authenticate to URL {0}", startUri));

            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    startUri,
                                                    endUri);

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                this.LogToConsole(string.Format("Received success message with content:\n{0}", result.ResponseData));
                var data = result.ResponseData.Substring(result.ResponseData.IndexOf('#'));
                var values = data.Split('&');
                var token = values[0].Split('=')[1];
                var expirationSeconds = values[1].Split('=')[1];
                var expiration = DateTime.UtcNow.AddSeconds(int.Parse(expirationSeconds));
                this.LogToConsole(string.Format("Saving token {0} with expiration UTC {1}", token, expiration));
                this.dataStorage.Save(this.Name, expiration, token);
                return token;
            }

            this.LogToConsole("Authentication failed.");

            throw new SecurityException(string.Format("Authentication failed: {0}", result.ResponseErrorDetail));
        }
    }
}
