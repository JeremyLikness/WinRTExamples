// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The design view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewModelLocatorExample
{
    using System;

    /// <summary>
    /// The design view model.
    /// </summary>
    public class DesignViewModel : IClockViewModel
    {
        /// <summary>
        /// Gets the time.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}