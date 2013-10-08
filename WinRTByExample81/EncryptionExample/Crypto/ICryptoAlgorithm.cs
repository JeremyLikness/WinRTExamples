// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICryptoAlgorithm.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The CryptoAlgorithm interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Crypto
{
    using System.Threading.Tasks;

    /// <summary>
    /// The CryptoAlgorithm interface.
    /// </summary>
    public interface ICryptoAlgorithm
    {
        /// <summary>
        /// Gets a value indicating whether is symmetric.
        /// </summary>
        bool IsSymmetric { get; }

        /// <summary>
        /// Gets a value indicating whether the algorithm includes authentication
        /// </summary>
        bool HasAuthentication { get; }

        /// <summary>
        /// Gets a value indicating whether the algorithm is for encryption 
        /// </summary>
        bool IsEncryptionAlgorithm { get; }

        /// <summary>
        /// Gets a value indicating whether the algorithm is for signing 
        /// </summary>
        bool IsVerificationAlgorithm { get; }

        /// <summary>
        /// Gets a value indicating whether it is a simple hash
        /// </summary>
        bool IsHash { get; }

        /// <summary>
        /// Gets a value indicating whether is block.
        /// </summary>
        bool IsBlock { get; }

        /// <summary>
        /// Gets a value indicating whether is PKSC #7.
        /// </summary>
        bool IsPkcs7 { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the mode of operation
        /// </summary>
        ModeOfOperation ModeOfOperation { get; }

        /// <summary>
        /// Sign the data
        /// </summary>
        /// <param name="key">Key to use for signing</param>
        /// <param name="data">Data to sign</param>
        /// <returns>The signature</returns>
        string Sign(string key, string data);

        /// <summary>
        /// Verify the signature is valid
        /// </summary>
        /// <param name="key">The key to use for signing</param>
        /// <param name="data">The data</param>
        /// <param name="signature">The signature</param>
        /// <returns>True if the data is verified</returns>
        bool Verify(string key, string data, string signature);

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        string Encrypt(string key, string data);

        /// <summary>
        /// The decrypt.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        string Decrypt(string key, string data);
    }
}