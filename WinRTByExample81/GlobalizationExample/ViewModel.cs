namespace GlobalizationExample
{
    using System;
    using System.Collections.Generic;

    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Resources;
    using Windows.Globalization.NumberFormatting;

    public class ViewModel
    {
        private const double Number = Math.PI;

        private readonly ResourceLoader resourceLoader;

        private readonly string geographicRegion;

        private readonly IReadOnlyList<string> languages;
        

        public ViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                this.resourceLoader = new ResourceLoader("Resources");
                this.geographicRegion = "US";
                this.languages = new[] { "en-US" };
            }
            else
            {
                this.resourceLoader = ResourceLoader.GetForCurrentView();
                this.geographicRegion = Windows.System.UserProfile.GlobalizationPreferences.HomeGeographicRegion;
                this.languages = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            }
        }

        public string SimpleDate
        {
            get
            {
                return "2014-02-08";
            }
        }

        public string FormattedNumber
        {
            get
            {
                var decimalFormatter =
                    new DecimalFormatter(this.languages, this.geographicRegion);
                var percentageFormatter = new PercentFormatter(this.languages, this.geographicRegion);
                return string.Format(
                    this.resourceLoader.GetString("FormattedNumber"),
                    decimalFormatter.FormatDouble(Number),
                    percentageFormatter.FormatDouble(Number));
            }
        }
    }
}