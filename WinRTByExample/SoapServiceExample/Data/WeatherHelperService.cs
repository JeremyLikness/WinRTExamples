// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeatherHelperService.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   Defines the WeatherHelperService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using SoapServiceExample.WeatherService;

    /// <summary>
    /// The weather description service.
    /// </summary>
    public class WeatherHelperService : IWeatherHelper
    {
        /// <summary>
        /// The _weather.
        /// </summary>
        private readonly List<WeatherDescription> weather = new List<WeatherDescription>();
        
        /// <summary>
        /// The get weather for zip code.
        /// </summary>
        /// <param name="zipCode">
        /// The zip code.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<WeatherForecast> GetWeatherForZipCode(string zipCode)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return new DesignForecast();
            }

            var factory = new ChannelFactory<WeatherSoapChannel>(
                new BasicHttpBinding(), 
                new EndpointAddress("http://wsf.cdyne.com/WeatherWS/Weather.asmx"));
            var channel = factory.CreateChannel();
            var forecast = await channel.GetCityForecastByZIPAsync(zipCode);
            var result = forecast.AsWeatherForecast();

            foreach (var day in result.Forecast)
            {
                day.ForecastUri = await this.GetImageUriForType(day.TypeId);
            }
            
            return result; 
        }

        /// <summary>
        /// The get image uri for type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/>.
        /// </returns>
        private async Task<Uri> GetImageUriForType(int type)
        {
            if (!this.weather.Any())
            {
                var proxy = new WeatherSoapClient();
                var result = await proxy.GetWeatherInformationAsync();
                foreach (var item in result.GetWeatherInformationResult)
                {
                    this.weather.Add(item);
                }
            }

            var uri = this.weather.Where(d => d.WeatherID == type)
                .Select(d => new Uri(d.PictureURL, UriKind.Absolute))
                .FirstOrDefault();

            return uri;
        }
    }
}