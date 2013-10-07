// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICanLog.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The CanLog interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AuthenticationExample.Data
{
    using System;

    /// <summary>
    /// The CanLog interface.
    /// </summary>
    public interface ICanLog
    {
        /// <summary>
        /// Gets or sets the log to console method
        /// </summary>
        Action<string> LogToConsole { get; set; }
    }
}