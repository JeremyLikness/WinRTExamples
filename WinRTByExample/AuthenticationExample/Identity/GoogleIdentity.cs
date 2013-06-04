// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleIdentity.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The live identity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Identity
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using AuthenticationExample.Data;

    using Windows.Data.Json;

    // https://gusclass.com/blog/2012/11/20/pulling-google-data-into-windows-store-apps/

    /// <summary>
    /// The live identity.
    /// </summary>
    public class GoogleIdentity : IOnlineIdentity, ICanLog 
    {
        /// <summary>
        /// The identity url.
        /// </summary>
        private const string IdentityUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleIdentity"/> class.
        /// </summary>
        public GoogleIdentity()
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
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", accessToken));

            this.LogToConsole(string.Format("Attempting to get identity from URL {0}.", IdentityUrl));

            HttpResponseMessage response = await client.GetAsync(IdentityUrl);
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                this.LogToConsole("Unabel to retrieve identity.");
                throw new InvalidOperationException("Unable to retrieve the profile from Google.");
            }

            this.LogToConsole(string.Format("Received content:\n{0}", content));
            return JsonObject.Parse(content)["email"].GetString();
        }
    }
}