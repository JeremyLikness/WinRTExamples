// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using EncryptionExample.Common;
    using EncryptionExample.Crypto;

    using Windows.Security.Cryptography.Core;
    using Windows.UI.Popups;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : BindableBase, ICommand  
    {
        /// <summary>
        /// The algorithms.
        /// </summary>
        private readonly List<ICryptoAlgorithm> algorithms = new List<ICryptoAlgorithm>();

        /// <summary>
        /// The verification algorithms
        /// </summary>
        private bool verification;

        /// <summary>
        /// The hash algorithms
        /// </summary>
        private bool hash;

        /// <summary>
        /// The symmetric.
        /// </summary>
        private bool symmetric;

        /// <summary>
        /// The asymmetric.
        /// </summary>
        private bool asymmetric;

        /// <summary>
        /// The block.
        /// </summary>
        private bool block;

        /// <summary>
        /// The stream.
        /// </summary>
        private bool stream;

        /// <summary>
        /// The PKCS#7.
        /// </summary>
        private bool pkcs7;

        /// <summary>
        /// The authenticated algorithms
        /// </summary>
        private bool authenticated;

        /// <summary>
        /// The selected algorithm.
        /// </summary>
        private ICryptoAlgorithm selectedAlgorithm;

        /// <summary>
        /// The encryption input.
        /// </summary>
        private string encryptionInput;

        /// <summary>
        /// The decryption input.
        /// </summary>
        private string decryptionInput;

        /// <summary>
        /// The key input.
        /// </summary>
        private string keyInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            // symmetric algorithms
            this.algorithms.Add(new Crypto(true, "Advanced Encryption Standard: Cipher Block Chaining", SymmetricAlgorithmNames.AesCbc));
            this.algorithms.Add(new Crypto(true, "Advanced Encryption Standard: Electronic Code Book", SymmetricAlgorithmNames.AesEcb));
            this.algorithms.Add(new Crypto(true, "Data Encryption Standard: Cipher Block Chaining", SymmetricAlgorithmNames.DesCbc));
            this.algorithms.Add(new Crypto(true, "Rivest Cipher v2: Electronic Code Book", SymmetricAlgorithmNames.Rc2Ecb));
            this.algorithms.Add(new Crypto(true, "Triple Data Encryption Standard: Cipher Block Chaining", SymmetricAlgorithmNames.TripleDesCbc));
            
            // symmetric algorithms with PKCS#7
            this.algorithms.Add(new Crypto(true, "Advanced Encryption Standard: Cipher Block Chaining with PKCS#7", SymmetricAlgorithmNames.AesCbcPkcs7));
            this.algorithms.Add(new Crypto(true, "Data Encryption Standard: Cipher Block Chaining with PKCS#7", SymmetricAlgorithmNames.DesCbcPkcs7));
            this.algorithms.Add(new Crypto(true, "Rivest Cipher v2: Cipher Block Chaining with PKCS#7", SymmetricAlgorithmNames.Rc2CbcPkcs7));
            this.algorithms.Add(new Crypto(true, "Triple Data Encryption Standard: Cipher Block Chaining with PKCS#7", SymmetricAlgorithmNames.TripleDesCbcPkcs7));
            
            // authenticated symmetric algorithms
            this.algorithms.Add(new Crypto(true, "Advanced Encryption Standard: Galois/Counter Mode", SymmetricAlgorithmNames.AesGcm));
            this.algorithms.Add(new Crypto(true, "Advanced Encryption Standard: Counter with CBC-MAC", SymmetricAlgorithmNames.AesCcm));

            // stream cipher
            this.algorithms.Add(new Crypto(true, "Rivest Cipher v4 (RC4)", SymmetricAlgorithmNames.Rc4));            
            
            // asymmetric algorithm example
            this.algorithms.Add(new Crypto(false, "Ron Rivest, Adi Shamir, and Leonard Adleman (RSA) PKCS#1", AsymmetricAlgorithmNames.RsaPkcs1));
            
            // hash algorithms
            this.algorithms.Add(new Crypto(true, "Message Digest 5 (MD5)", HashAlgorithmNames.Md5));
            this.algorithms.Add(new Crypto(true, "Secure Hash Algorithm 1 (SHA1)", HashAlgorithmNames.Sha1));
            this.algorithms.Add(new Crypto(true, "Secure Hash Algorithm 256-bit (SHA2)", HashAlgorithmNames.Sha256));
            this.algorithms.Add(new Crypto(true, "Secure Hash Algorithm 512-bit (SHA2)", HashAlgorithmNames.Sha512));

            // MAC algorithms
            this.algorithms.Add(new Crypto(true, "Hash-based Message Authentication Code with Message Digest", MacAlgorithmNames.HmacMd5));
            this.algorithms.Add(new Crypto(true, "Hash-based Message Authentication Code with Secure Hash Algorithm 1", MacAlgorithmNames.HmacSha1));
            this.algorithms.Add(new Crypto(true, "Hash-based Message Authentication Code with Secure Hash Algorithm 256-bit (SHA2)", MacAlgorithmNames.HmacSha256));
            this.algorithms.Add(new Crypto(true, "Hash-based Message Authentication Code with Secure Hash Algorithm 512-bit (SHA2)", MacAlgorithmNames.HmacSha512));

            this.selectedAlgorithm = this.algorithms[0];
            this.encryptionInput = "Overwrite this with the text you wish to secure.";
            this.decryptionInput = "Overwrite this with the encrypted text you wish to decode or the signature to verify.";
        }

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Gets or sets a value indicating whether symmetric.
        /// </summary>
        public bool Symmetric
        {
            get
            {
                return this.symmetric;
            }

            set
            {
                this.symmetric = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether asymmetric.
        /// </summary>
        public bool Asymmetric
        {
            get
            {
                return this.asymmetric;
            }

            set
            {
                this.asymmetric = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether requires password
        /// </summary>
        public bool RequiresPassword
        {
            get
            {
                return this.selectedAlgorithm != null && this.selectedAlgorithm.IsSymmetric
                       && !this.selectedAlgorithm.IsHash;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether authenticated.
        /// </summary>
        public bool Authenticated
        {
            get
            {
                return this.authenticated;
            }

            set
            {
                this.authenticated = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether block.
        /// </summary>
        public bool Block
        {
            get
            {
                return this.block;
            }

            set
            {
                this.block = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether stream.
        /// </summary>
        public bool Stream
        {
            get
            {
                return this.stream;
            }

            set
            {
                this.stream = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether verification.
        /// </summary>
        public bool Verification
        {
            get
            {
                return this.verification;
            }

            set
            {
                this.verification = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether hash.
        /// </summary>
        public bool Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.hash = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PKCS#7.
        /// </summary>
        public bool Pkcs7
        {
            get
            {
                return this.pkcs7;
            }

            set
            {
                this.pkcs7 = value;
                this.OnPropertyChanged();
                this.OnFilterChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected algorithm.
        /// </summary>
        public ICryptoAlgorithm SelectedAlgorithm
        {
            get
            {
                return this.selectedAlgorithm;
            }

            set
            {
                this.selectedAlgorithm = value;
                this.OnPropertyChanged();
// ReSharper disable ExplicitCallerInfoArgument
                this.OnPropertyChanged("RequiresPassword");
// ReSharper restore ExplicitCallerInfoArgument
            }
        }

        /// <summary>
        /// Gets or sets the encryption input.
        /// </summary>
        public string EncryptionInput
        {
            get
            {
                return this.encryptionInput;
            }

            set
            {
                this.encryptionInput = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the encryption input.
        /// </summary>
        public string DecryptionInput
        {
            get
            {
                return this.decryptionInput;
            }

            set
            {
                this.decryptionInput = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the encryption input.
        /// </summary>
        public string KeyInput
        {
            get
            {
                return this.keyInput;
            }

            set
            {
                this.keyInput = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the algorithms.
        /// </summary>
        public IEnumerable<ICryptoAlgorithm> Algorithms
        {
            get
            {
                return this.algorithms
                    .Where(a => !this.pkcs7 || a.IsPkcs7)
                    .Where(a => !this.block || a.IsBlock)
                    .Where(a => !this.stream || !a.IsBlock)
                    .Where(a => !this.symmetric || a.IsSymmetric)
                    .Where(a => !this.asymmetric || !a.IsSymmetric)
                    .Where(a => !this.authenticated || a.HasAuthentication)
                    .Where(a => !this.verification || a.IsVerificationAlgorithm)
                    .Where(a => !this.hash || a.IsHash)
                    .OrderBy(a => a.Name)
                    .ToList();                
            }
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return parameter != null && this.selectedAlgorithm != null;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public async void Execute(object parameter)
        {
            if (!this.CanExecute(parameter))
            {
                return;
            }

            if (parameter.Equals("encrypt"))
            {
                await this.Encrypt();
            }
            else if (parameter.Equals("decrypt"))
            {
                await this.Decrypt();
            }
            else if (parameter.Equals("sign"))
            {
                await this.Sign();
            }
            else if (parameter.Equals("verify"))
            {
                await this.Verify();
            }
        }

        /// <summary>
        /// The show error.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <remarks>
        /// For testing this can be placed in an interface and assigned, i.e. IDialog, so in implementation
        /// the dialog is shown but in testing just the call is verified
        /// </remarks>
        private static async Task ShowDialog(string title, string message)
        {
            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Sign()
        {
            if (string.IsNullOrEmpty(this.encryptionInput))
            {
                await ShowDialog("Missing Required Input", "The text to sign is required.");
                return;
            }

            if (this.selectedAlgorithm.IsSymmetric && !this.selectedAlgorithm.IsHash && string.IsNullOrEmpty(this.keyInput))
            {
                await ShowDialog("Missing Required Input", "The password is required.");
                return;
            }

            var title = "Success";
            var message = "Text was successfully signed.";

            try
            {
                this.DecryptionInput = this.selectedAlgorithm.Sign(this.keyInput, this.encryptionInput);                                            
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            await ShowDialog(title, message);
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Verify()
        {
            if (string.IsNullOrEmpty(this.encryptionInput))
            {
                await ShowDialog("Missing Required Input", "The text to verify is required.");
                return;
            }

            if (this.selectedAlgorithm.IsSymmetric && !this.selectedAlgorithm.IsHash && string.IsNullOrEmpty(this.keyInput))
            {
                await ShowDialog("Missing Required Input", "The password is required.");
                return;
            }

            var title = "Verification complete.";
            string message;

            try
            {
                message = this.selectedAlgorithm.Verify(
                            this.keyInput, this.encryptionInput, this.decryptionInput)
                                        ? "The signature is valid."
                                        : "The signature is not valid: the text or signature may have been tampered with.";                
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            await ShowDialog(title, message);
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Encrypt()
        {
            if (string.IsNullOrEmpty(this.encryptionInput))
            {
                await ShowDialog("Missing Required Input", "The text to encrypt is required.");
                return;
            }

            if (this.selectedAlgorithm.IsSymmetric && string.IsNullOrEmpty(this.keyInput))
            {
                await ShowDialog("Missing Required Input", "The password is required.");
                return;
            }

            var title = "Success";
            var message = "Text was successfully encrypted.";

            try
            {
                this.DecryptionInput = this.selectedAlgorithm.Encrypt(this.keyInput, this.encryptionInput);
                this.EncryptionInput = string.Empty;                
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            await ShowDialog(title, message);
        }

        /// <summary>
        /// The encrypt.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Decrypt()
        {
            if (string.IsNullOrEmpty(this.decryptionInput))
            {
                await ShowDialog("Missing Required Input", "The data to decrypt is required.");
                return;
            }

            if (this.selectedAlgorithm.IsSymmetric && string.IsNullOrEmpty(this.keyInput))
            {
                await ShowDialog("Missing Required Input", "The password is required.");
                return;
            }

            var title = "Success";
            var message = "Data was successfully decrypted.";

            try
            {
                this.EncryptionInput = this.selectedAlgorithm.Decrypt(this.keyInput, this.decryptionInput);
                this.DecryptionInput = string.Empty;                
            }
            catch (Exception ex)
            {
                title = "Error";
                message = ex.Message;
            }

            await ShowDialog(title, message);
        }

        /// <summary>
        /// The on filter changed.
        /// </summary>
        private void OnFilterChanged()
        {
// ReSharper disable ExplicitCallerInfoArgument
            this.OnPropertyChanged("Algorithms");
// ReSharper restore ExplicitCallerInfoArgument
        }
    }
}