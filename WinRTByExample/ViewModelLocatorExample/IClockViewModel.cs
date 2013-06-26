// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClockViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The ClockViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewModelLocatorExample
{
    using System;

    /// <summary>
    /// The ClockViewModel interface.
    /// </summary>
    public interface IClockViewModel
    {
        /// <summary>
        /// Gets the time.
        /// </summary>
        DateTime Time { get; } 
    }
}