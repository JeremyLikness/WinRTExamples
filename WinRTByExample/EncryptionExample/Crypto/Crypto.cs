// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crypto.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The crypto.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Crypto
{
    /// <summary>
    /// The crypto.
    /// </summary>
    public class Crypto : BaseCrypto
    {
        /// <summary>
        /// The is symmetric.
        /// </summary>
        private readonly bool isSymmetric;

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The algorithm name.
        /// </summary>
        private readonly string algorithmName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Crypto"/> class.
        /// </summary>
        /// <param name="isSymmetric">
        /// The is symmetric.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="algorithmName">
        /// The algorithm name.
        /// </param>
        public Crypto(bool isSymmetric, string name, string algorithmName)
        {
            this.isSymmetric = isSymmetric;
            this.name = name;
            this.algorithmName = algorithmName;
        }

        /// <summary>
        /// Gets a value indicating whether is symmetric.
        /// </summary>
        public override bool IsSymmetric
        {
            get
            {
                return this.isSymmetric;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the algorithm name.
        /// </summary>
        protected override string AlgorithmName
        {
            get
            {
                return this.algorithmName;
            }
        }
    }
}
