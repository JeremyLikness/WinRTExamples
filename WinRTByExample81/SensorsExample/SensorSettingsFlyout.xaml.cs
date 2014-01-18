using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace SensorsExample
{
    public sealed partial class SensorSettingsFlyout : SettingsFlyout
    {
        public SensorSettingsFlyout()
        {
            Settings = ((App)Application.Current).SensorSettings;
            InitializeComponent();
        }

        public SensorSettings Settings { get; private set; }
    }
}
