using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769
using ShareTargetExample.Common;

namespace ShareTargetExample
{
    public sealed partial class AboutSettingsFlyout : SettingsFlyout
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        public AboutSettingsFlyout()
        {
            this.InitializeComponent();
            
            DefaultViewModel["Name"] = Package.Current.DisplayName;
            DefaultViewModel["Version"] = Package.Current.Id.Version.DisplayText();
            DefaultViewModel["Description"] = Package.Current.Description;
        }
    }

    public static partial class Extensions
    {
        public static String DisplayText(this PackageVersion version)
        {
            return String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
