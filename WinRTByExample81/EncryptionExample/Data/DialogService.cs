// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The dialog service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExample.Data
{
    using System;
    using System.Threading.Tasks;

    using Windows.UI.Popups;

    /// <summary>
    /// The dialog service.
    /// </summary>
    public class DialogService : IDialogService
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
        public async Task ShowDialog(string title, string message)
        {
            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }
    }
}