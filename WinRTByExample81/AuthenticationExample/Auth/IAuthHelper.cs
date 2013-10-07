// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The AuthHelper interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Auth
{
    using System.Threading.Tasks;

    /// <summary>
    /// The Authentication Helper interface.
    /// </summary>
    public interface IAuthHelper 
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The authenticate async.
        /// </summary>
        /// <param name="claims">
        /// The claims.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> AuthenticateAsync(string[] claims);        
    }
}