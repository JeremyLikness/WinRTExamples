// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignForecast.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The design forecast.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System;

    /// <summary>
    /// The design forecast.
    /// </summary>
    public class DesignForecast : WeatherForecast
    {
        /// <summary>
        /// The test uris.
        /// </summary>
        private readonly Uri[] testUris = new[]
                                        {
                                            new Uri(
                                                "http://ws.cdyne.com/WeatherWS/Images/mostlycloudy.gif", 
                                                UriKind.Absolute),
                                            new Uri(
                                                "http://ws.cdyne.com/WeatherWS/Images/sunny.gif", 
                                                UriKind.Absolute)
                                        };

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignForecast"/> class.
        /// </summary>
        public DesignForecast()
        {
            this.City = "Woodstock";
            this.State = "GA"; 
            this.Result = "Design-mode data";
            
            for (var x = 0; x < 7; x++)
            {
                var offset = 7 - x;
                var entry = new ForecastEntry
                {
                    Day = DateTime.Now.AddDays(-1 * offset),
                    ForecastUri = this.testUris[x % 2],
                    Description = string.Format("Rainy {0}", x),
                    PrecipitationDay = "50",
                    PrecipitationNight = "20",
                    TemperatureLow = "25",
                    TemperatureHigh = "49",
                    TypeId = x
                };
                this.Forecast.Add(entry);
            }
        }
    }
}