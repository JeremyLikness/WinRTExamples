// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleAuthenticator.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The live authenticator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Auth
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using AuthenticationExample.Data;

    using Windows.Data.Json;
    using Windows.Security.Authentication.Web;

    /// <summary>
    /// The live authenticator.
    /// </summary>
    public class GoogleAuthenticator : IAuthHelper, ICanLog 
    {
        /// <summary>
        /// Client id
        /// </summary>
        private const string ClientId = "{InsertYourSecretNumber}.apps.googleusercontent.com"; // https://code.google.com/apis/console/ API Access 

        /// <summary>
        /// The client secret.
        /// </summary>
        private const string ClientSecret = "{PasteYourSecret}"; // https://code.google.com/apis/console/ API Access

        /// <summary>
        /// Redirect to local
        /// </summary>
        private const string RedirectUrl = "http://localhost";

        /// <summary>
        /// URL to get the token
        /// </summary>
        private const string GoogleUrl =
            "https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope={2}";

        /// <summary>
        /// The authentication url.
        /// </summary>
        private const string OAuthUrl = "https://accounts.google.com/o/oauth2/token";

        /// <summary>
        /// The authentication post.
        /// </summary>
        private const string OAuthPost =
            "code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code";

        /// <summary>
        /// Claim (we want info about the profile)
        /// </summary>
        private const string IdentityClaim = "https://www.googleapis.com/auth/userinfo.{0}";

        /// <summary>
        /// Data storage provider
        /// </summary>
        private readonly ICredentialStorage dataStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAuthenticator"/> class.
        /// </summary>
        /// <param name="dataStorage">
        /// The data storage.
        /// </param>
        public GoogleAuthenticator(ICredentialStorage dataStorage)
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
                return "Google";
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
        /// <exception cref="SecurityException">Thrown when authentication fails 
        /// </exception>
        public async Task<string> AuthenticateAsync(string[] claims)
        {
            string savedToken;
            if (this.dataStorage.TryRestore(this.Name, out savedToken))
            {
                this.LogToConsole(string.Format("Using saved token: {0}", savedToken));
                return savedToken;
            }

            var claimsExpanded = claims.Select(claim => string.Format(IdentityClaim, claim)).ToArray();

            var startUri = new Uri(
                string.Format(GoogleUrl, ClientId, RedirectUrl, string.Join(",", claimsExpanded)), UriKind.Absolute);

            this.LogToConsole(string.Format("Attemping to authenticate to URL {0}", startUri));

            WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                startUri,
                new Uri(RedirectUrl, UriKind.Absolute));

            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                this.LogToConsole("Successfully authenticated.");
                var results = result.ResponseData.Split('=');
                var code = results[1];

                this.LogToConsole(string.Format("Received Google code {0}", code));

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, OAuthUrl);

                this.LogToConsole(string.Format("Attempting to retreive token from {0}", OAuthUrl));

                var content = string.Format(OAuthPost, code, ClientId, ClientSecret, RedirectUrl);

                this.LogToConsole(string.Format("Request content: \n{0}", content));

                request.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(content)));
                request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    this.LogToConsole(
                        string.Format("Received content from authentication post: \n{0}", responseContent));

                    var jsonObject = JsonObject.Parse(responseContent);
                    var accessToken = jsonObject["access_token"].GetString();
                    var expires = jsonObject["expires_in"].GetNumber();
                    var expiration = DateTime.UtcNow.AddSeconds(expires);

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        this.LogToConsole(string.Format("Saved access token {0} with expiration UTC {1}.", accessToken, expiration));
                        this.dataStorage.Save(this.Name, expiration, accessToken);
                        return accessToken;
                    }

                    this.LogToConsole("No access token found.");
                    throw new SecurityException("Authentication failed: unable to extract the access token.");
                }

                this.LogToConsole("No response received");
                throw new SecurityException("Authentication failed: no response from the server.");
            }

            this.LogToConsole("Authentication attempt was not successful.");
            throw new SecurityException(string.Format("Authentication failed: {0}", result.ResponseErrorDetail));
        }        
    }
}