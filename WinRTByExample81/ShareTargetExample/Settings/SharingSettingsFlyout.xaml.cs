using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace ShareTargetExample
{
    public sealed partial class SharingSettingsFlyout : SettingsFlyout
    {
        private readonly AppSettings _appSettings = new AppSettings();
        private readonly ObservableCollection<ShareFormat> _orderedShareFormats;

        public SharingSettingsFlyout()
        {
            InitializeComponent();

            AcceptAllSwitch.IsOn = !_appSettings.AcceptAllSetting;

            _orderedShareFormats = new ObservableCollection<ShareFormat>(_appSettings.OrderedFormats);

            // Tie in changes to the list in settings to storage
            _orderedShareFormats.CollectionChanged += (sender, args) =>
                                                      {
                                                          _appSettings.UpdateFormatOrder(_orderedShareFormats);
                                                      };
            MyTestListView.ItemsSource = _orderedShareFormats;
        }

        private void HandleAcceptAllSwitchToggled(Object sender, RoutedEventArgs e)
        {
            _appSettings.AcceptAllSetting = !AcceptAllSwitch.IsOn;
        }
    }
}
