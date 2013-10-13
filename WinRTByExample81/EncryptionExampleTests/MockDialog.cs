// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockDialog.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The mock dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EncryptionExampleTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using EncryptionExample.Data;

    /// <summary>
    /// The mock dialog.
    /// </summary>
    public class MockDialog : IDialogService 
    {
        /// <summary>
        /// The dialogs.
        /// </summary>
        private readonly List<Tuple<string, string>> dialogs = new List<Tuple<string, string>>();

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the dialogs.
        /// </summary>
        public IEnumerable<Tuple<string, string>> Dialogs
        {
            get
            {
                return this.dialogs;
            }
        }

        /// <summary>
        /// Gets the dialog count.
        /// </summary>
        public int DialogCount
        {
            get
            {
                return this.dialogs.Count;
            }
        }

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
        public Task ShowDialog(string title, string message)
        {
            this.Title = title;
            this.Message = message;
            this.dialogs.Add(Tuple.Create(title, message));
            return Task.Run(() => { });
        }
    }
}