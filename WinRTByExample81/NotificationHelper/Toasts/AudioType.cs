// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The audio type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Toasts
{
    /// <summary>
    /// The audio type.
    /// </summary>
    public class AudioType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioType"/> class.
        /// </summary>
        /// <param name="displayType">
        /// The display type.
        /// </param>
        /// <param name="fullType">
        /// The full type.
        /// </param>
        /// <param name="loopCapable">
        /// The loop capable.
        /// </param>
        public AudioType(string displayType, string fullType, bool loopCapable)
        {
            this.DisplayType = displayType;
            this.FullType = fullType;
            this.LoopCapable = loopCapable;
        }

        /// <summary>
        /// Gets the display type
        /// </summary>
        public string DisplayType { get; private set; }

        /// <summary>
        /// Gets the full type used for setting toasts
        /// </summary>
        public string FullType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the type can e looped 
        /// </summary>
        public bool LoopCapable { get; private set; }
    }
}
