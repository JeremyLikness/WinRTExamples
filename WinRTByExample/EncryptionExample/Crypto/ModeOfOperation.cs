// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModeOfOperation.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The mode of operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Crypto
{
    /// <summary>
    /// The mode of operation.
    /// </summary>
    public enum ModeOfOperation
    {
        /// <summary>
        /// The Cipher-block chaining mode of operation.
        /// </summary>
        CBC,

        /// <summary>
        /// The Electronic Codebook mode of operation.
        /// </summary>
        ECB,

        /// <summary>
        /// The Galois Counter Mode of operation
        /// </summary>
        GCM,

        /// <summary>
        /// The counter with CBC-MAC mode of operation
        /// </summary>
        CCM,

        /// <summary>
        /// Not applicable
        /// </summary>
        NA
    }
}