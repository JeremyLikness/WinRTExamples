// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using SoapServiceExample.WeatherService;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// The as forecast entry.
        /// </summary>
        /// <param name="forecast">
        /// The forecast.
        /// </param>
        /// <returns>
        /// The <see cref="ForecastEntry"/>.
        /// </returns>
        public static ForecastEntry AsForecastEntry(this Forecast forecast)
        {
            return new ForecastEntry
            {
                Day = forecast.Date,
                Description = forecast.Desciption,
                PrecipitationDay = forecast.ProbabilityOfPrecipiation.Daytime,
                PrecipitationNight = forecast.ProbabilityOfPrecipiation.Nighttime,
                TemperatureLow = forecast.Temperatures.MorningLow,
                TemperatureHigh = forecast.Temperatures.DaytimeHigh,
                TypeId = forecast.WeatherID
            };
        }

        /// <summary>
        /// The as weather forecast.
        /// </summary>
        /// <param name="forecastReturn">
        /// The forecast return.
        /// </param>
        /// <returns>
        /// The <see cref="WeatherForecast"/>.
        /// </returns>
        public static WeatherForecast AsWeatherForecast(this ForecastReturn forecastReturn)
        {
            var result = new WeatherForecast
                             {
                                 City = forecastReturn.WeatherStationCity,
                                 State = forecastReturn.State,
                                 Result = forecastReturn.ResponseText
                             };

            if (forecastReturn.Success)
            {
                foreach (var day in forecastReturn.ForecastResult)
                {
                    result.Forecast.Add(day.AsForecastEntry());
                }
            }

            return result;
        }
    }
}
