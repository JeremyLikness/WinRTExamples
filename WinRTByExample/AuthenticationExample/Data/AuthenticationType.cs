// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Type of authentication to use
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Data
{
    using AuthenticationExample.Auth;
    using AuthenticationExample.Identity;

    /// <summary>
    /// Type of authentication to use 
    /// </summary>
    public class AuthenticationType
    {
        /// <summary>
        /// Gets or sets the name of the authenticator 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identity service
        /// </summary>
        public IOnlineIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the authentication service
        /// </summary>
        public IAuthHelper Auth { get; set; }
    }
}