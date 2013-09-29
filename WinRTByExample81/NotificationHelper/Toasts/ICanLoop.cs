// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICanLoop.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   This is simply a decorator
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Toasts
{
    /// <summary>
    /// This is simply a decorator 
    /// </summary>
    public interface ICanLoop 
    {
        /// <summary>
        /// Gets a value indicating whether loop capable.
        /// </summary>
        bool LoopCapable { get; }
    }
}