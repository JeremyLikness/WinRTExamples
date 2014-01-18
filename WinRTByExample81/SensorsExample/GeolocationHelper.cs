using System;
using System.Collections.Generic;
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

    public static class GeocoordinateExtensions
    {
        public static List<String> ToDataDump([NotNull] this Geocoordinate geocoordinate)
        {
            if (geocoordinate == null) throw new ArgumentNullException("geocoordinate");

            var results = new List<String>();
            results.Add(String.Format("Position Source: {0}", geocoordinate.PositionSource));
            results.Add(String.Format("Timestamp: {0:h:mm:ss tt}", geocoordinate.Timestamp));
            results.Add(String.Format("Accuracy: {0} meters", geocoordinate.Accuracy));
            results.Add(String.Format("Altitude Accuracy: {0} meters", geocoordinate.AltitudeAccuracy));
            results.Add(String.Format("Speed: {0} meters/sec", geocoordinate.Speed));
            results.Add(String.Format("Heading: {0} degrees", geocoordinate.Heading));
            results.Add("Point:");
            results.Add(String.Format("    GeoshapeType: {0}", geocoordinate.Point.GeoshapeType));
            results.Add("    Position:");
            results.Add(String.Format("        Latitude: {0} degrees", geocoordinate.Point.Position.Latitude));
            results.Add(String.Format("        Longitude: {0} degrees", geocoordinate.Point.Position.Longitude));
            results.Add(String.Format("        Altitude: {0}", geocoordinate.Point.Position.Altitude));
            results.Add(String.Format("    Altitude Reference System: {0}", geocoordinate.Point.AltitudeReferenceSystem));
            results.Add(String.Format("    SpatialReferenceId: {0}", geocoordinate.Point.SpatialReferenceId));
            results.Add("Satellite Data:");
            results.Add(String.Format("    HorizontalDilutionOfPrecision: {0}", geocoordinate.SatelliteData.HorizontalDilutionOfPrecision));
            results.Add(String.Format("    VerticalDilutionOfPrecision: {0}", geocoordinate.SatelliteData.VerticalDilutionOfPrecision));
            results.Add(String.Format("    PositionDilutionOfPrecision: {0}", geocoordinate.SatelliteData.PositionDilutionOfPrecision));

            // Deprecated values: 
            // geocoordinate.Latitude
            // geocoordinate.Longitude
            // geocoordinate.Altitude
            

            return results;
        }
    }
}