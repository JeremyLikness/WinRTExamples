// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppCredentialStorage.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The app data storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Data
{
    using System;

    using Windows.Security.Credentials;

    /// <summary>
    /// The app data storage.
    /// </summary>
    public class AppCredentialStorage : ICredentialStorage
    {
        /// <summary>
        /// The username.
        /// </summary>
        private const string Username = "Current";

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="expiration">
        /// The expiration for the token
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        public void Save(string key, DateTime expiration, string token)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(key, Username, string.Format("{0}={1}", expiration, token)));
        }

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
        public bool TryRestore(string key, out string value)
        {
            value = string.Empty;
            var vault = new PasswordVault();

            PasswordCredential credential;

            try
            {
                credential = vault.Retrieve(key, Username);
            }
            catch (Exception)
            {
                return false;
            }

            if (credential != null)
            {
                var values = credential.Password.Split('=');
                var expiration = DateTime.SpecifyKind(DateTime.Parse(values[0]), DateTimeKind.Utc);

                if (expiration > DateTime.UtcNow)
                {
                    value = values[1];
                    return true;
                }
            }

            return false;
        }
    }
}