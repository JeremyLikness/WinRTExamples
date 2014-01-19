using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public static class GeolocationExtensions
    {
        public static String DisplayText(this BasicGeoposition reading, Boolean includeAltitude)
        {
            return includeAltitude
                ? String.Format("Lat: {0} Lon:{1} Alt:{2}",
                    reading.Latitude,
                    reading.Longitude,
                    reading.Altitude)
                : String.Format("Lat: {0} Lon:{1}",
                    reading.Latitude,
                    reading.Longitude);
        }

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