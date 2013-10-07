// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnlineIdentity.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Liknss
// </copyright>
// <summary>
//   The OnlineIdentity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Identity
{
    using System.Threading.Tasks;

    /// <summary>
    /// The OnlineIdentity interface.
    /// </summary>
    public interface IOnlineIdentity
    {
        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <param name="accessToken">
        /// The access Token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> GetEmail(string accessToken);
    }
}