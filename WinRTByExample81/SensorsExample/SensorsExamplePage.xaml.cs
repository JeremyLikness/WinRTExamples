using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using SensorsExample.Annotations;
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
        private readonly GeofenceHelper _geofenceHelper;

        private readonly DispatcherTimer _timer = new DispatcherTimer(); 

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorsExamplePage"/> class.
        /// </summary>
        public SensorsExamplePage()
        {
            _sensorSettings = ((App)Application.Current).SensorSettings;
            _sensorHelper = new SensorHelper(_sensorSettings);
            _geolocationHelper = new GeolocationHelper(_sensorSettings);
            _geofenceHelper = new GeofenceHelper();
            _geofenceHelper.FenceUpdated += HandleGeofenceHelperFenceUpdated;
            _geofenceHelper.FenceRemoved += HandleGeofenceHelperFenceRemoved;

            InitializeComponent();

            AddGeofencesToMap();

            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += HandleNavigationHelperLoadState;
            _navigationHelper.SaveState += HandleNavigationHelperSaveState;
        }

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
            DefaultViewModel["ZoomLevel"] = Math.Max(ExampleMap.MinZoomLevel, 2.0);
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


        #endregion

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

        #region Geolocation

        private async void HandleShowCurentLocationRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var coordinate = await _geolocationHelper.GetCoordinate();
            if (coordinate != null)
            {
                DumpDataList.ItemsSource = coordinate.ToDataDump();
            }
            _timer.Start();
        }

        private async void HandleCenterOnLocationRequest(Object sender, RoutedEventArgs e)
        {
            // Suspend the inclinometer timer while recentering to avoid a potential race condition
            _timer.Stop();
            var coordinate = await _geolocationHelper.GetCoordinate();
            if (coordinate != null)
            {
                DefaultViewModel["Position"] = coordinate.Point.Position;
            }
            _timer.Start();
        } 

        #endregion

        #region Geofencing

        public void HandleFenceLocationRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            
            var position = (BasicGeoposition) DefaultViewModel["Position"];

            var flyout = (Flyout) FlyoutBase.GetAttachedFlyout((FrameworkElement) sender);
            var flyoutContent = (FrameworkElement) flyout.Content;

            var geofenceViewModel = new GeofenceItemViewModel(_geofenceHelper, position);
            flyoutContent.DataContext = geofenceViewModel;
            geofenceViewModel.FenceAdded += (o, args) =>
                                            {
                                                flyout.Hide();
                                                AddGeofenceToMap(args.Geofence);
                                            };
            flyout.ShowAt((FrameworkElement) sender);

            _timer.Start();
        }

        public void HandleFenceListingRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            
            var flyout = (Flyout) FlyoutBase.GetAttachedFlyout((FrameworkElement) sender);
            var flyoutContent = (FrameworkElement) flyout.Content;

            var geofenceListingViewModel = new GeofenceListingViewModel(_geofenceHelper);
            geofenceListingViewModel.FenceRemoved += (o, args) => RemoveGeofenceFromMap(args.Geofence.Id);
            flyoutContent.DataContext = geofenceListingViewModel;
            flyout.ShowAt((FrameworkElement) sender);
            
            _timer.Start();
        }


        private async void HandleGeofenceHelperFenceUpdated(Object sender, FenceUpdateEventArgs args)
        {
            Debug.WriteLine("Fence {0} updated - {1} at {2:G}.  Latitude = {3}, Longitude = {4}",
                args.FenceId,
                args.Reason,
                args.Timestamp,
                args.Position.Latitude,
                args.Position.Longitude);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var fenceUpdateMessage =
                    String.Format("Fence {0} updated - {1} at {2:G}.  Latitude = {3}, Longitude = {4}",
                        args.FenceId,
                        args.Reason,
                        args.Timestamp,
                        args.Position.Latitude,
                        args.Position.Longitude);
                await ShowMessage(fenceUpdateMessage);
            });
        }

        private async void HandleGeofenceHelperFenceRemoved(Object sender, FenceRemovedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                RemoveGeofenceFromMap(args.FenceId);
                var fenceUpdateMessage =
                    String.Format("Fence {0} removed - {1}",
                        args.FenceId,
                        args.WhyRemoved);
                await ShowMessage(fenceUpdateMessage);
            });
        }

        private readonly Dictionary<String, MapElements> _mapFenceElements = new Dictionary<String, MapElements>();

        private class MapElements
        {
            public MapPolygon Circle { get; set; }
            public Pushpin Pin { get; set; }
        }

        private void AddGeofencesToMap()
        {
            foreach (var fence in _geofenceHelper.GetCurrentFences())
            {
                AddGeofenceToMap(fence);
            }
        }

        private void AddGeofenceToMap([NotNull] Geofence fence)
        {
            if (fence == null) throw new ArgumentNullException("fence");

            var geocircle = (Geocircle)fence.Geoshape;
            var circle = new MapPolygon
            {
                FillColor = Color.FromArgb(128, 0, 0, 255),
                Locations = GetCircleLocations(geocircle.Center, geocircle.Radius)
            };

            // Get the first shape layer for the circles, or add a new one to accomodate
            var shapeLayer = ExampleMap.ShapeLayers.FirstOrDefault();
            if (shapeLayer == null)
            {
                shapeLayer = new MapShapeLayer();
                ExampleMap.ShapeLayers.Add(shapeLayer);
            }
            shapeLayer.Shapes.Add(circle);

            var pushpin = new Pushpin();
            pushpin.Tapped += (sender, args) => ShowPinMessage(fence);

            MapLayer.SetPosition(pushpin, new Location(geocircle.Center.Latitude, geocircle.Center.Longitude));
            ExampleMap.Children.Add(pushpin);

            _mapFenceElements.Add(fence.Id, new MapElements { Circle = circle, Pin = pushpin });
        }

        private void RemoveGeofenceFromMap(String fenceId)
        {
            MapElements mapElements;
            if (_mapFenceElements.TryGetValue(fenceId, out mapElements))
            {
                var shapeLayer = ExampleMap.ShapeLayers.FirstOrDefault();
                if (shapeLayer != null)
                {
                    shapeLayer.Shapes.Remove(mapElements.Circle);
                }
                ExampleMap.Children.Remove(mapElements.Pin);
            }
        }

        private LocationCollection GetCircleLocations(BasicGeoposition center, Double radiusInMeters)
        {
            const Double earthRadiusInMeters = 6367 * 1000;
            var centerLatitudeRadians = center.Latitude * Math.PI / 180;
            var centerLongitudeRadians = center.Longitude * Math.PI / 180;

            var angularCoverage = radiusInMeters / earthRadiusInMeters;
            var locations = new LocationCollection();
            for (var x = 0; x <= 360; x++)
            {
                var radian = x * Math.PI / 180;
                var latitudeRadians =
                    Math.Asin(Math.Sin(centerLatitudeRadians) * Math.Cos(angularCoverage) +
                              Math.Cos(centerLatitudeRadians) * Math.Sin(angularCoverage) * Math.Cos(radian));
                var longitudeRadians = centerLongitudeRadians +
                                       Math.Atan2(
                                           Math.Sin(radian) * Math.Sin(angularCoverage) * Math.Cos(centerLatitudeRadians),
                                           Math.Cos(angularCoverage) - Math.Sin(centerLatitudeRadians) * Math.Sin(centerLatitudeRadians)
                                           );
                locations.Add(new Location(latitudeRadians * (180 / Math.PI), longitudeRadians * (180 / Math.PI)));
            }
            return locations;
        }

        private async void ShowPinMessage([NotNull] Geofence geofence)
        {
            if (geofence == null) throw new ArgumentNullException("geofence");

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var id = geofence.Id;
                var circle = (Geocircle)geofence.Geoshape;
                await ShowMessage(String.Format("Fence {0} centered at Latitude {1} degrees, Longitude {2} degrees, of Radius {3} meters.",
                            id,
                            circle.Center.Latitude,
                            circle.Center.Longitude,
                            circle.Radius));
            });
        }

        #endregion

        #region Sensors

        private async void HandleCompassRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetCompassReading();
            await ShowMessage(String.Format("Compass: {0}", _sensorSettings.GetCompassReadingDisplayText(reading)));
            _timer.Start();
        }

        private async void HandleInclinometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetInclinometerReading();
            await ShowMessage(String.Format("Inclinometer: {0}", _sensorSettings.GetInclinometerReadingDisplayText(reading)));
            _timer.Start();
        }

        private async void HandleSimpleOrientationRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetSimpleOrientationReading();
            await ShowMessage(String.Format("SimpleOrientation = {0}", reading));
            _timer.Start();
        }

        private async void HandleAccelerometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetAccelerometerReading();
            await ShowMessage(String.Format("Accelerometer: {0}", _sensorSettings.GetAccelerometerReadingDisplayText(reading)));
            _timer.Start();
        }

        private async void HandleGyrometerRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetGyrometerReading();
            await ShowMessage(String.Format("Gyrometer: {0}", _sensorSettings.GetGyrometerReadingDisplayText(reading)));
            _timer.Start();
        }

        private async void HandleOrientationSensorRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetOrientationSensorReading();
            await ShowMessage(String.Format("Orientation Sensor: {0}", _sensorSettings.GetOrientationSensorDisplayText(reading)));
            _timer.Start();
        }

        private async void HandleLightSensorRequest(Object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            var reading = _sensorHelper.GetLightSensorReading();
            await ShowMessage(String.Format("Light Sensor: {0}", _sensorSettings.GetLightSensorDisplayText(reading)));
            _timer.Start();
        }

        #endregion

        #region Animation Timer

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

                // Optionally normalize the sensor reading values
                var displayAdjustment
                    = _sensorSettings.CompensateForDisplayOrientation
                        ? _sensorSettings.DisplayOrientation.AxisAdjustmentFactor()
                        : SensorExtensions.AxisOffset.Default;
                var adjustedPitchDegrees
                    = inclinometerReading.PitchDegrees * displayAdjustment.X;
                var adjustedRollDegrees
                    = inclinometerReading.RollDegrees * displayAdjustment.Y;

                // At full speed/inclination, move 100% map size per tick
                const Double maxScreensPerTick = 1.00;
                var mapWidth = ExampleMap.ActualWidth;
                var xFullRateTraversalPerTick = mapWidth * maxScreensPerTick;
                var mapHeight = ExampleMap.ActualHeight;
                var yFullRateTraversalPerTick = mapHeight * maxScreensPerTick;

                // Turn rotation angles into percentages
                var xTraversalPercentage
                    = Math.Sin(adjustedRollDegrees * Math.PI / 180);
                var yTraversalPercentage
                    = Math.Sin(adjustedPitchDegrees * Math.PI / 180);

                // Compute the final traversal amounts based on the percent-ages
                // and compute the new destination center point
                var xTraversalAmount
                    = xTraversalPercentage * xFullRateTraversalPerTick;
                var yTraversalAmount
                    = yTraversalPercentage * yFullRateTraversalPerTick;
                var destinationPoint = new Point(
                    mapWidth / 2 + xTraversalAmount,
                    mapHeight / 2 + yTraversalAmount);

                // Use the Bing Maps methods to convert pixel pos to Lat/Lon
                // rather than trying to figure out Mercator map math
                Location location;
                if (ExampleMap.TryPixelToLocation(destinationPoint, out location))
                {
                    // Obtain the current map position (for altitude)
                    var position = (BasicGeoposition)DefaultViewModel["Position"];

                    var newPosition = new BasicGeoposition
                    {
                        Altitude = position.Altitude,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };

                    DefaultViewModel["Position"] = newPosition;
                }
            }

            if (_sensorSettings.IsFollowingCompass)
            {
                // Get the latest compass reading
                var compassReading = _sensorSettings.LatestCompassReading;

                // Adjust the reading based on the display orientation, if necessary
                var displayOffset = _sensorSettings.CompensateForDisplayOrientation
                    ? _sensorSettings.DisplayOrientation.CompassOffset()
                    : 0;
                var heading
                    = (compassReading.HeadingMagneticNorth + displayOffset) % 360;

                // Set the value used by data binding to update the map's heading
                DefaultViewModel["Heading"] = heading;
            }

            _timer.Start();
        } 

        #endregion

        #region Message Display

        private readonly Queue<String> _queuedMessages = new Queue<String>();
        private Boolean _isShowingMessageDialog;

        /// <summary>
        /// Shows a message dialog with the given message text.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <remarks>
        /// Because there are circumstanecs where the sensors are hooked up to show message information (especially Geofencing)
        /// and they can do so asynchronously (especially Geofencing), it is conceivable to run into a problem where a second
        /// request to show a message dialog occurs when one is already being displayed (especially Geofencing).  This is one of 
        /// those proverbial "very bad things", as it results in an access denied exception.  The solution seen here basically 
        /// queues any message requests that arrive while one box is showing.  When a box is done being shown, the queue gets 
        /// checked, and if any messages are in the queue, a new box is shown.
        /// </remarks>
        private async Task ShowMessage(String message)
        {
            _queuedMessages.Enqueue(message);
            await ShowPendingMessages();
        }

        private async Task ShowPendingMessages()
        {
            if (_isShowingMessageDialog || !_queuedMessages.Any()) return;

            _isShowingMessageDialog = true;
            var messageDialog = new MessageDialog(_queuedMessages.Dequeue(), "Sensors");
            await messageDialog.ShowAsync();

            _isShowingMessageDialog = false;
            await ShowPendingMessages();
        } 

        #endregion
    }
}
