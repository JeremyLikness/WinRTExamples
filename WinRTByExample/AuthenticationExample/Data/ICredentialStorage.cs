// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStorage.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The DataStorage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Data
{
    using System;

    /// <summary>
    /// The DataStorage interface.
    /// </summary>
    public interface ICredentialStorage
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="expiration">
        /// The expiration.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        void Save(string key, DateTime expiration, string token);

        /// <summary>
        /// Sign out (destroy the credential)
        /// </summary>
        /// <param name="key">The key</param>
        void Signout(string key);

        /// <summary>
        /// The try restore.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool TryRestore(string key, out string value);
    }
}