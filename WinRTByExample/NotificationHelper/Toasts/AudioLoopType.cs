// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioLoopType.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The audio loop type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WinRTByExample.NotificationHelper.Toasts
{
    /// <summary>
    /// The audio loop type - just a decorated class to make it easier to determine looping capability
    /// </summary>
    public class AudioLoopType : AudioType, ICanLoop
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioLoopType"/> class.
        /// </summary>
        /// <param name="displayType">
        /// The display type.
        /// </param>
        /// <param name="fullType">
        /// The full type.
        /// </param>
        public AudioLoopType(string displayType, string fullType) : base(displayType, fullType, true)
         {      
         }
    }
}