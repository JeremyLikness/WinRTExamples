// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TemplatesExample
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// The random generator.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// The messages.
        /// </summary>
        private readonly string[] messages =
            {
                "Something happened.", "What's going on?", "Is there any memory remaining?",
                "Code has executed.", "Something was implemented.", "This is totally random."
            };

        /// <summary>
        /// The message types.
        /// </summary>
        private readonly MessageType[] messageTypes = { MessageType.Information, MessageType.Warning, MessageType.Error };

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            this.AddMessage = new AddMessageCommand(this.GenerateMessage);
            this.Messages = new ObservableCollection<MessageInstance>();

            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return;
            }

            for (var x = 0; x < 5; x++)
            {
                this.AddMessage.Execute(null);
            }
        }

        /// <summary>
        /// Gets the add message command.
        /// </summary>
        public ICommand AddMessage { get; private set; }

        /// <summary>
        /// Gets the list of messages.
        /// </summary>
        public ObservableCollection<MessageInstance> Messages { get; private set; }

        /// <summary>
        /// Gets a random item from a list.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <typeparam name="T">The type of the list
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/> random item from the list
        /// </returns>
        private T GetRandomItem<T>(IList<T> list)
        {
            return list[this.random.Next(list.Count)];
        }

        /// <summary>
        /// Generate a new message.
        /// </summary>
        private void GenerateMessage()
        {
            this.Messages.Add(
                new MessageInstance { Message = this.GetRandomItem(this.messages), Type = this.GetRandomItem(this.messageTypes) });
        }
    }
}
