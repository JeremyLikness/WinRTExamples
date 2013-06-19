// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCrypto.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The base crypto.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Crypto
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;

    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;

    /// <summary>
    /// The base crypto.
    /// </summary>
    public abstract class BaseCrypto : ICryptoAlgorithm
    {
        /// <summary>
        /// The encoding.
        /// </summary>
        private const BinaryStringEncoding Encoding = BinaryStringEncoding.Utf8;

        /// <summary>
        /// The initialization vector.
        /// </summary>
        private const string Iv = "Initialization Vector";

        /// <summary>
        /// The authentication data.
        /// </summary>
        private const string AuthenticationData = "Secret Data";

        /// <summary>
        /// The hash names.
        /// </summary>
        private readonly List<string> hashNames = new List<string>
                                                      {
                                                          HashAlgorithmNames.Md5, 
                                                          HashAlgorithmNames.Sha1,
                                                          HashAlgorithmNames.Sha256, 
                                                          HashAlgorithmNames.Sha384,
                                                          HashAlgorithmNames.Sha512
                                                      };

        /// <summary>
        /// The mac names.
        /// </summary>
        private readonly List<string> macNames = new List<string> 
        {
                                                     MacAlgorithmNames.AesCmac, 
                                                     MacAlgorithmNames.HmacMd5,
                                                     MacAlgorithmNames.HmacSha1, 
                                                     MacAlgorithmNames.HmacSha256,
                                                     MacAlgorithmNames.HmacSha384, 
                                                     MacAlgorithmNames.HmacSha512
                                                 };

        /// <summary>
        /// The key pair.
        /// </summary>
        private CryptographicKey keyPair;

        /// <summary>
        /// Gets a value indicating whether is symmetric.
        /// </summary>
        public abstract bool IsSymmetric { get; }

        /// <summary>
        /// Gets a value indicating whether is encryption algorithm.
        /// </summary>
        public bool IsEncryptionAlgorithm
        {
            get
            {
                return !this.IsVerificationAlgorithm;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is verification algorithm.
        /// </summary>
        public bool IsVerificationAlgorithm
        {
            get
            {
                if (this.IsSymmetric)
                {
                    return this.AlgorithmName.StartsWith("MD5") || this.AlgorithmName.StartsWith("SHA")
                           || this.AlgorithmName.StartsWith("HMAC") || this.hashNames.Contains(this.AlgorithmName)
                           || this.macNames.Contains(this.AlgorithmName);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it is a hash 
        /// </summary>
        public bool IsHash
        {
            get
            {
                return this.hashNames.Contains(this.AlgorithmName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is block.
        /// </summary>
        public bool IsBlock
        {
            get
            {
                return this.IsSymmetric && !this.AlgorithmName.Equals(SymmetricAlgorithmNames.Rc4);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm has authentication.
        /// </summary>
        public bool HasAuthentication
        {
            get
            {
                return this.AlgorithmName.Equals(SymmetricAlgorithmNames.AesGcm)
                       || this.AlgorithmName.Equals(SymmetricAlgorithmNames.AesCcm);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is PKCS#7.
        /// </summary>
        public bool IsPkcs7
        {
            get
            {
                return this.AlgorithmName.Contains("PKCS7");
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the mode of operation.
        /// </summary>
        public ModeOfOperation ModeOfOperation
        {
            get
            {
                if (this.AlgorithmName.Contains("CBC"))
                {
                    return ModeOfOperation.CBC;                    
                }

                if (this.AlgorithmName.Contains("CCM"))
                {
                    return ModeOfOperation.CCM;
                }

                if (this.AlgorithmName.Contains("ECB"))
                {
                    return ModeOfOperation.ECB;
                }

                if (this.AlgorithmName.Contains("GCM"))
                {
                    return ModeOfOperation.GCM;                    
                }

                return ModeOfOperation.NA;                
            }
        }
        
        /// <summary>
        /// Gets the algorithm name.
        /// </summary>
        protected abstract string AlgorithmName { get; }

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
        public string Encrypt(string key, string data)
        {
            IBuffer message = CryptographicBuffer.ConvertStringToBinary(data, Encoding);

            if (this.IsSymmetric)
            {
                IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, Encoding);
                return this.EncryptSymmetric(keyBuffer, message);
            }

            return this.EncryptAsymmetric(message);
        }

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
        public string Decrypt(string key, string data)
        {
            if (this.IsSymmetric)
            {
                IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, Encoding);
                return this.DecryptSymmetric(keyBuffer, data);
            }

            return this.DecryptAsymmetric(data);
        }

        /// <summary>
        /// Sign the data
        /// </summary>
        /// <param name="key">Key to use for signing</param>
        /// <param name="data">Data to sign</param>
        /// <returns>The signature</returns>
        public string Sign(string key, string data)
        {
            return this.IsSymmetric ? this.SignSymmetric(key, data) : this.SignAsymmetric(data);
        }

        /// <summary>
        /// Verify the signature is valid
        /// </summary>
        /// <param name="key">The key to use for signing</param>
        /// <param name="data">The data</param>
        /// <param name="signature">The signature</param>
        /// <returns>True if the data is verified</returns>
        public bool Verify(string key, string data, string signature)
        {
            return this.IsSymmetric ? this.VerifySymmetric(key, data, signature) : this.VerifyAsymmetric(data, signature);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Crypto instance: {0}", this.Name);
        }

        /// <summary>
        /// Get the nonce
        /// </summary>
        /// <returns>The nonce</returns>
        private static IBuffer GetNonce()
        {
            return
                CryptographicBuffer.CreateFromByteArray(
                    new byte[] { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb });
        }

        /// <summary>
        /// The ensure length.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="withRepeat">
        /// The with repeat.
        /// </param>
        /// <returns>
        /// The <see cref="IBuffer"/>.
        /// </returns>
        private static IBuffer EnsureLength(IBuffer buffer, int length, bool withRepeat)
        {
            if (buffer.Length == length)
            {
                return buffer;
            }

            if (buffer.Length > length)
            {
                var array = new byte[length];
                buffer.CopyTo(0, array, 0, length);
                return array.AsBuffer();
            }

            var longerArray = new byte[length];
            if (!withRepeat)
            {
                buffer.CopyTo(0, longerArray, 0, (int)buffer.Length);
                return longerArray.AsBuffer();
            }

            var pos = 0;
            while (pos < length)
            {
                var toGo = length - pos;
                var pad = toGo <= buffer.Length ? toGo : (int)buffer.Length;
                buffer.CopyTo(0, longerArray, pos, pad);
                pos += pad;
            }

            return longerArray.AsBuffer();
        }

        /// <summary>
        /// The encrypt asymmetric.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EncryptAsymmetric(IBuffer message)
        {
            var algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
            if (this.keyPair == null)
            {
                this.keyPair = algorithm.CreateKeyPair(512);
            }

            var publicKey = algorithm.ImportPublicKey(this.keyPair.ExportPublicKey());
            var encryptedMessage = CryptographicEngine.Encrypt(publicKey, message, null);
            return CryptographicBuffer.EncodeToBase64String(encryptedMessage);
        }

        /// <summary>
        /// The encrypt symmetric.
        /// </summary>
        /// <param name="keyBuffer">
        /// The key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string EncryptSymmetric(IBuffer keyBuffer, IBuffer message)
        {
            IBuffer initializationBuffer = null;

            var algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);

            if (!this.IsPkcs7)
            {
                var pad = message.Length % algorithm.BlockLength;
                if (pad > 0)
                {
                    var desiredLength = message.Length + algorithm.BlockLength - pad;
                    message = EnsureLength(message, (int)desiredLength, false);
                }
            }

            CryptographicKey keyMaterial = algorithm.CreateSymmetricKey(EnsureLength(keyBuffer, 32, true));

            if (this.ModeOfOperation.Equals(ModeOfOperation.CBC))
            {
                var initialationVector = CryptographicBuffer.ConvertStringToBinary(Iv, Encoding);
                initializationBuffer = EnsureLength(initialationVector, (int)algorithm.BlockLength, true);
            }

            if (this.HasAuthentication)
            {
                var authenticationData = CryptographicBuffer.ConvertStringToBinary(AuthenticationData, Encoding);
                EncryptedAndAuthenticatedData encryptedData = CryptographicEngine.EncryptAndAuthenticate(
                    keyMaterial, message, GetNonce(), authenticationData);
                return string.Format(
                    "{0}|{1}",
                    CryptographicBuffer.EncodeToBase64String(encryptedData.AuthenticationTag),
                    CryptographicBuffer.EncodeToBase64String(encryptedData.EncryptedData));
            }

            var encryptedBuffer = CryptographicEngine.Encrypt(keyMaterial, message, initializationBuffer);
            return CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
        }

        /// <summary>
        /// The decrypt asymmetric.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DecryptAsymmetric(string message)
        {
            IBuffer messageData = CryptographicBuffer.DecodeFromBase64String(message);
            var decryptedMessage = CryptographicEngine.Decrypt(this.keyPair, messageData, null);
            return CryptographicBuffer.ConvertBinaryToString(Encoding, decryptedMessage);
        }

        /// <summary>
        /// The decrypt symmetric.
        /// </summary>
        /// <param name="keyBuffer">
        /// The key buffer.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DecryptSymmetric(IBuffer keyBuffer, string data)
        {
            IBuffer initializationBuffer = null;
            IBuffer authenticationTag = null;
            IBuffer message;

            if (this.HasAuthentication)
            {
                var parts = data.Split('|');
                if (parts.Length != 2)
                {
                    throw new Exception("Missing authentication tag, encrypted data, or both.");
                }

                authenticationTag = CryptographicBuffer.DecodeFromBase64String(parts[0]);
                message = CryptographicBuffer.DecodeFromBase64String(parts[1]);
            }
            else
            {
                message = CryptographicBuffer.DecodeFromBase64String(data);
            }

            var algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);

            CryptographicKey keyMaterial = algorithm.CreateSymmetricKey(EnsureLength(keyBuffer, 32, true));

            if (this.ModeOfOperation.Equals(ModeOfOperation.CBC))
            {
                var initialationVector =
                    CryptographicBuffer.ConvertStringToBinary(Iv, Encoding);
                initializationBuffer = EnsureLength(
                    initialationVector, (int)algorithm.BlockLength, true);
            }

            if (this.HasAuthentication)
            {
                var authenticationData = CryptographicBuffer.ConvertStringToBinary(AuthenticationData, Encoding);
                var decryptedData = CryptographicEngine.DecryptAndAuthenticate(
                    keyMaterial, message, GetNonce(), authenticationTag, authenticationData);
                return CryptographicBuffer.ConvertBinaryToString(Encoding, decryptedData);
            }

            var decryptedBuffer = CryptographicEngine.Decrypt(
                keyMaterial, message, initializationBuffer);
            return CryptographicBuffer.ConvertBinaryToString(Encoding, decryptedBuffer);            
        }

        /// <summary>
        /// Sign symmetric data 
        /// </summary>
        /// <param name="key">The key (not needed for a hash)</param>
        /// <param name="data">The data</param>
        /// <returns>The signature or hash</returns>
        private string SignSymmetric(string key, string data)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, Encoding);
                
            if (this.hashNames.Contains(this.AlgorithmName))
            {
                var algorithm = HashAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
                IBuffer hash = algorithm.HashData(buffer);
                return CryptographicBuffer.EncodeToBase64String(hash);
            }

            var macAlgorithm = MacAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, Encoding);
            CryptographicKey keyMaterial = macAlgorithm.CreateKey(keyBuffer);
            var signature = CryptographicEngine.Sign(keyMaterial, buffer);
            return CryptographicBuffer.EncodeToBase64String(signature);
        }

        /// <summary>
        /// The verify symmetric.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="signature">
        /// The signature.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool VerifySymmetric(string key, string data, string signature)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, Encoding);
            IBuffer signatureBuffer = CryptographicBuffer.DecodeFromBase64String(signature);

            if (this.hashNames.Contains(this.AlgorithmName))
            {
                var algorithm = HashAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
                IBuffer hash = algorithm.HashData(buffer);
                return CryptographicBuffer.Compare(hash, signatureBuffer);
            }

            var macAlgorithm = MacAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
            IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, Encoding);
            var keyMaterial = macAlgorithm.CreateKey(keyBuffer);
            return CryptographicEngine.VerifySignature(keyMaterial, buffer, signatureBuffer);
        }

        /// <summary>
        /// The sign asymmetric.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SignAsymmetric(string data)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, Encoding);
            var algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
            if (this.keyPair == null)
            {
                this.keyPair = algorithm.CreateKeyPair(1024);
            }

            var publicKey = algorithm.ImportPublicKey(this.keyPair.ExportPublicKey());
            var signature = CryptographicEngine.Sign(publicKey, buffer);
            return CryptographicBuffer.EncodeToBase64String(signature);
        }

        /// <summary>
        /// The verify asymmetric.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="signature">
        /// The signature.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool VerifyAsymmetric(string data, string signature)
        {
            if (this.keyPair == null)
            {
                throw new Exception("Must sign to generate the key pair first.");
            }

            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(data, Encoding);
            IBuffer signatureBuffer = CryptographicBuffer.DecodeFromBase64String(signature);
            var algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(this.AlgorithmName);
            
            var keys = algorithm.ImportKeyPair(this.keyPair.Export());
            return CryptographicEngine.VerifySignature(keys, buffer, signatureBuffer);
        }
    }
}