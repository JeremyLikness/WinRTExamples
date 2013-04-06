// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherForecast.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The weather forecast.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// The weather forecast.
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecast"/> class.
        /// </summary>
        public WeatherForecast()
        {
            this.Forecast = new List<ForecastEntry>();
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets the forecast.
        /// </summary>
        public List<ForecastEntry> Forecast { get; private set; }
    }
}