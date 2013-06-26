// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignForecastEntry.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The design forecast entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System;

    /// <summary>
    /// The design forecast entry.
    /// </summary>
    public class DesignForecastEntry : ForecastEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignForecastEntry"/> class.
        /// </summary>
        public DesignForecastEntry()
        {
            this.Day = DateTime.Now;
            this.ForecastUri = new Uri("http://ws.cdyne.com/WeatherWS/Images/mostlycloudy.gif", UriKind.Absolute);
            this.Description = "Sample day for weather";
            this.PrecipitationDay = "50";
            this.PrecipitationNight = "20";
            this.TemperatureLow = "25";
            this.TemperatureHigh = "49";
            this.TypeId = 1;
        }
    }
}