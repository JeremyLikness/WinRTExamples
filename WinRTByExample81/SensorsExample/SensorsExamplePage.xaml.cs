using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SensorsExample.Common;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SensorsExample
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SensorsExamplePage : Page
    {

        #region Fields

        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly SensorSettings _sensorSettings;
        private readonly SensorHelper _sensorHelper;
        private readonly GeolocationHelper _geolocationHelper;

        private readonly DispatcherTimer _timer = new DispatcherTimer(); 

        #endregion

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorsExamplePage"/> class.
        /// </summary>
        public SensorsExamplePage()
        {
            _sensorSettings = new SensorSettings
                              {
                                  AccelerometerReportInterval = 16,
                                  CompassReportInterval = 16,
                                  GyrometerReportInterval = 16,
                                  InclinometerReportInterval = 16,
                                  LightSensorReportInterval = 16,
                                  OrientationSensorReportInterval = 16
                              };
            _sensorHelper = new SensorHelper(_sensorSettings);
            _geolocationHelper = new GeolocationHelper(_sensorSettings);

            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
            _navigationHelper.SaveState += HandleNavigationHelperSaveState;
        } 

        #endregion


        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void HandleNavigationHelperLoadState(Object sender, LoadStateEventArgs e)
        {
            DefaultViewModel["SensorSettings"] = _sensorSettings;
            DefaultViewModel["Position"] = new BasicGeoposition();
            InitializeTimer();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void HandleNavigationHelperSaveState(Object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void HandleShowCurentLocationRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var position = await _geolocationHelper.GetPosition();
            await ShowMessage(String.Format("Location: {0}",
                position.Coordinate.Point.Position.DisplayText()));
            _timer.Start();
        }

        private async void HandleCenterOnLocationRequest(Object sender, RoutedEventArgs e)
        {
            // Suspend the inclinometer timer while recentering to avoid a potential race condition
            _timer.Stop();

            var position = await _geolocationHelper.GetPosition();
            await Dispatcher.Dispatch(async () =>
            {
                if (position == null)
                {
                    await ShowMessage("No Position returned.");
                }
                else
                {
                    DefaultViewModel["Position"] = position.Coordinate.Point.Position;
                }
            });
            _timer.Start();
        }

        private async void HandleCompassRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetCompassReading();
            await ShowMessage(String.Format("Compass: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async void HandleInclinometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetInclinometerReading();
            await ShowMessage(String.Format("Inclinometer: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async void HandleSimpleOrientationRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetSimpleOrientationReaading();
            await ShowMessage(String.Format("SimpleOrientation = {0}", reading));
            _timer.Start();
        }

        private async void HandleAccelerometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetAccelerometerReading();
            await ShowMessage(String.Format("Accelerometer: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async void HandleGyrometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetGyrometerReading();
            await ShowMessage(String.Format("Gyrometer: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async void HandleOrientationSensorRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetOrientationSensorReading();
            await ShowMessage(String.Format("Orientation Sensor: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async void HandleLightSensorRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetLightSensorReading();
            await ShowMessage(String.Format("Light Sensor: {0}", reading.DisplayText()));
            _timer.Start();
        }

        private async Task ShowMessage(String message)
        {
            var messageDialog = new MessageDialog(message, "Sensors");
            await messageDialog.ShowAsync();
        }

        private void InitializeTimer()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(30);
            _timer.Start();
            _timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(Object sender, Object o)
        {
            _timer.Stop();
            if (_sensorSettings.FollowInclinometer)
            {
                var inclinometerReading = _sensorSettings.LatestInclinometerReading;

                // Adjust the rate of travel relative to the current map zoom level
                var position = (BasicGeoposition)DefaultViewModel["Position"];
                var factor = 50 / ExampleMap.ZoomLevel;
                var newlatitude = (position.Latitude + (factor * Math.Sin(inclinometerReading.PitchDegrees * Math.PI / 180))).Between(-90, 90);
                var newlongitude = (position.Longitude + (factor * Math.Sin(inclinometerReading.RollDegrees * Math.PI / 180))) % 180;


                var newPosition = new BasicGeoposition
                {
                    Altitude = position.Altitude,
                    Latitude = newlatitude,
                    Longitude = newlongitude
                };
                DefaultViewModel["Position"] = newPosition;
            }

            if (_sensorSettings.IsFollowingCompass)
            {
                var compassReading = _sensorSettings.LatestCompassReading;
                DefaultViewModel["Heading"] = compassReading.HeadingMagneticNorth;
            }

            _timer.Start();
        }
    }

    public static class Extensions
    {
        public static Double Between(this Double value, Double minValue, Double maxValue)
        {
            if (minValue > maxValue) throw new InvalidOperationException("Max must be greater than min.");
            var result = Math.Min(value, maxValue);
            result = Math.Max(result, minValue);
            return result;
        }

        public static async Task Dispatch(this CoreDispatcher dispatcher, Action dispatchAction)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, dispatchAction.Invoke);
        }
    }
}
