using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public class GeolocationHelper
    {
        #region Fields

        private readonly SensorSettings _sensorSettings;
        private readonly Geolocator _geolocator;
        private Boolean _isGeolocatorReady;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GeolocationHelper"/> class.
        /// </summary>
        /// <param name="sensorSettings">The sensor settings.</param>
        public GeolocationHelper([NotNull] SensorSettings sensorSettings)
        {
            if (sensorSettings == null) throw new ArgumentNullException("sensorSettings");

            _sensorSettings = sensorSettings;
            // Listen for sensor property changes to update the accuracy request
            _sensorSettings.PropertyChanged += HandleSensorSettingsPropertyChanged;

            _geolocator = new Geolocator();
            
            // Listen for status change events, but also immediately get the status. 
            // This is in case it is already at its end-state and therefore 
            // won't generate a change event.
            _geolocator.StatusChanged +=
                (o, e) => SetGeoLocatorReady(e.Status == PositionStatus.Ready);
            SetGeoLocatorReady(_geolocator.LocationStatus == PositionStatus.Ready);

            // Set the desired accuracy.  Alternatively, can use 
            // DesiredAccuracyInMeters, where < 100 meters ==> high accuracy
            _geolocator.DesiredAccuracy = GetDesiredPositionAccuracy();

            // Listen for position changed events.  
            // Set to not report more often than once every 10 seconds 
            // and only when movement exceeds 50 meters
            _geolocator.ReportInterval = 10000; // Value in ms
            _geolocator.MovementThreshold = 50; // Value in meters
            _geolocator.PositionChanged += GeolocatorOnPositionChanged;
        }

        #endregion

        #region Geolocator Readiness / Availability
        private void SetGeoLocatorReady(Boolean isReady)
        {
            _sensorSettings.IsLocationAvailable = isReady;
            _isGeolocatorReady = isReady;
        } 
        #endregion

        #region Position Accuracy

        private void HandleSensorSettingsPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Set the disired accuracy value based on the current sensor setting.
            _geolocator.DesiredAccuracy = GetDesiredPositionAccuracy();
            // Alternatively, can use DesiredAccuracyInMeters, where < 100 meters ==> high accuracy
            //_geolocator.DesiredAccuracyInMeters = _sensorSettings.DesiredLocationAccuracyInMeters;
        }

        private PositionAccuracy GetDesiredPositionAccuracy()
        {
            return _sensorSettings.IsLocationRequestingHighAccuracy
                ? PositionAccuracy.High
                : PositionAccuracy.Default;
        }

        #endregion

        #region Getting the Position

        public async Task<Geocoordinate> GetCoordinate()
        {
            Geocoordinate location = null;
            if (_isGeolocatorReady)
            {
                var position = await _geolocator.GetGeopositionAsync();
                location = position.Coordinate;
            }
            return location;
        }

        private void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            _sensorSettings.LatestLocationReading = args.Position.Coordinate;
        } 

        #endregion
    }
}