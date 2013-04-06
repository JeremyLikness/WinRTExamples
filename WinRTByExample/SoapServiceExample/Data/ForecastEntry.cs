// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForecastEntry.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The forecast entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System;

    /// <summary>
    /// The forecast entry.
    /// </summary>
    public class ForecastEntry
    {
        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// Gets the day text.
        /// </summary>
        public string DayText
        {
            get
            {
                return this.Day.Date.ToString("D");
            }
        }

        /// <summary>
        /// Gets or sets the type of the forecast
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the precipitation day.
        /// </summary>
        public string PrecipitationDay { get; set; }

        /// <summary>
        /// Gets or sets the precipitation night.
        /// </summary>
        public string PrecipitationNight { get; set; }

        /// <summary>
        /// Gets or sets the temperature low.
        /// </summary>
        public string TemperatureLow { get; set; }

        /// <summary>
        /// Gets or sets the temperature high.
        /// </summary>
        public string TemperatureHigh { get; set; }

        /// <summary>
        /// Gets or sets the forecast uri.
        /// </summary>
        public Uri ForecastUri { get; set; }
    }
}
