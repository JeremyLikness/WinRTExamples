// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdProvider.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The id provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Skrape.Data
{
    /// <summary>
    /// The id provider.
    /// </summary>
    public class IdProvider
    {
        /// <summary>
        /// The _id.
        /// </summary>
        private int id;

        /// <summary>
        /// The register id method - ensures id is always large enough to avoid conflicts.
        /// </summary>
        /// <param name="idToRegister">
        /// The id to register.
        /// </param>
        public void RegisterId(int idToRegister)
        {
            if (idToRegister > this.id)
            {
                this.id = idToRegister;
            }
        }

        /// <summary>
        /// The get id.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetId()
        {
            return ++this.id;
        }
    }
}
