// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Type of the message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TemplatesExample
{
    /// <summary>
    /// Type of the message
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Message is information only.
        /// </summary>
        Information,

        /// <summary>
        /// Message is a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Message describes an error.
        /// </summary>
        Error
    }
}
