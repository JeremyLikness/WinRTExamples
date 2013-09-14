// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWeatherHelper.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Interface to help get the weather information
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to get the weather description information
    /// </summary>
    public interface IWeatherHelper
    {
        /// <summary>
        /// Get the forecast for a zip code
        /// </summary>
        /// <param name="zipCode">The zip code to forecast for</param>
        /// <returns>The forecast for the city the zip code is in</returns>
        Task<WeatherForecast> GetWeatherForZipCode(string zipCode);
    }
}