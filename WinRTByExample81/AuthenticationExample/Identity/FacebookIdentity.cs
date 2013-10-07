// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacebookIdentity.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The facebook identity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Identity
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using AuthenticationExample.Data;

    using Windows.Data.Json;

    /// <summary>
    /// The facebook identity.
    /// </summary>
    public class FacebookIdentity : IOnlineIdentity, ICanLog 
    {
        /// <summary>
        /// The facebook identity url.
        /// </summary>
        private const string FacebookIdentityUrl = "https://graph.facebook.com/me?scope=email";

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookIdentity"/> class.
        /// </summary>
        public FacebookIdentity()
        {
            this.LogToConsole = delegate { };
        }

        /// <summary>
        /// Gets or sets the log to console.
        /// </summary>
        public Action<string> LogToConsole { get; set; }

        /// <summary>
        /// The get email.
        /// </summary>
        /// <param name="accessToken">
        /// The access token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<string> GetEmail(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", accessToken);
            this.LogToConsole(string.Format("Attempting to access identity with URL {0}", FacebookIdentityUrl));
            var result = await client.GetStringAsync(FacebookIdentityUrl);
            this.LogToConsole(string.Format("Received content:\n{0}", result));
            var profileInformation = JsonObject.Parse(result);
            var email = profileInformation["email"].GetString();
            return email;
        }
    }
}