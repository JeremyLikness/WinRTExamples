// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The DialogService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Data
{
    using System.Threading.Tasks;

    /// <summary>
    /// The DialogService interface.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// The show dialog.
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
        Task ShowDialog(string title, string message);
    }
}