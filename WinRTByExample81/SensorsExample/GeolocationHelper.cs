using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public class GeolocationHelper
    {
        private readonly SensorSettings _sensorSettings;
        private readonly Geolocator _geolocator;
        private Boolean _isGeolocatorReady;

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GeolocationHelper"/> class.
        /// </summary>
        /// <param name="sensorSettings">The sensor settings.</param>
        public GeolocationHelper([NotNull] SensorSettings sensorSettings)
        {
            if (sensorSettings == null) throw new ArgumentNullException("sensorSettings");

            _sensorSettings = sensorSettings;

            _geolocator = new Geolocator();
            _geolocator.StatusChanged += GeolocatorOnStatusChanged;
            _geolocator.PositionChanged += GeolocatorOnPositionChanged;

            SetGeoLocatorReady(_geolocator.LocationStatus == PositionStatus.Ready);
        }

        #endregion

        public async Task<Geoposition> GetPosition()
        {
            Geoposition position = null;
            if (_isGeolocatorReady)
            {
                position = await _geolocator.GetGeopositionAsync();  
            }
            return position;

            // _geolocator.LocationStatus == PositionStatus.NotInitialized

            // _geolocator.DesiredAccuracy == PositionAccuracy.Default // PositionAccuracy.High
            // _geolocator.DesiredAccuracyInMeters
            
            // _geolocator.MovementThreshold   // distance that must be traversed to raise a PositionChanged event
            // _geolocator.ReportInterval =  // Minimum time interval (MS) for position updates 

            // position.Coordinate.Accuracy
            // position.Coordinate.Altitude
            // position.Coordinate.AltitudeAccuracy
            // position.Coordinate.Heading;
            // position.Coordinate.Speed
            // position.Coordinate.Point.AltitudeReferenceSystem == AltitudeReferenceSystem.Terrain
            // position.Coordinate.Point.GeoshapeType == GeoshapeType.Geocircle
            // position.Coordinate.Point.SpatialReferenceId
            // position.Coordinate.Point.Position.Altitude Latitide Longitude
            // position.Coordinate.PositionSource == PositionSource.WiFi Cellular IPAddress Satellite
            // position.Coordinate.SatelliteData

            // position.CivicAddress.City
            // position.CivicAddress.State
            // position.CivicAddress.Country
            // position.CivicAddress.PostalCode
        }

        private void SetGeoLocatorReady(Boolean isReady)
        {
            _sensorSettings.IsLocationAvailable = isReady;
            _isGeolocatorReady = isReady;
        }

        private void GeolocatorOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            SetGeoLocatorReady(args.Status == PositionStatus.Ready);
        }

        private void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            _sensorSettings.LatestLocationReading = args.Position.Coordinate.Point.Position;
        }
    }
}