// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Jeremy Likness">
//   Copyright (c) Jeremy Likness
// </copyright>
// <summary>
//   The view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SoapServiceExample
{
    using Common;
    using Data;

    /// <summary>
    /// The view model.
    /// </summary>
    public class ViewModel : NotifyBase
    {
        /// <summary>
        /// The weather helper.
        /// </summary>
        private readonly IWeatherHelper weatherHelper;

        /// <summary>
        /// The current forecast.
        /// </summary>
        private WeatherForecast currentForecast;

        /// <summary>
        /// The zip code.
        /// </summary>
        private string zipCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {            
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.currentForecast = new DesignForecast();
                this.zipCode = "30189";
                return;
            }

            this.weatherHelper = new WeatherHelperService();

            this.SubmitCommand = new WeatherCommand
            {
                ExecuteAction =
                    async zipCode =>
                    {
                        this.CurrentForecast =
                            await
                            this.weatherHelper.GetWeatherForZipCode(
                                zipCode);
                    }
            };
        }

        /// <summary>
        /// Gets the submit command.
        /// </summary>
        public WeatherCommand SubmitCommand { get; private set; }

        /// <summary>
        /// Gets or sets the current forecast.
        /// </summary>
        public WeatherForecast CurrentForecast
        {
            get
            {
                return this.currentForecast;
            }

            set
            {
                this.currentForecast = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        public string ZipCode
        {
            get
            {
                return this.zipCode;
            }

            set
            {
                this.zipCode = value;
                this.OnPropertyChanged();
                this.SubmitCommand.OnCanExecuteChange();
            }
        }
    }
}