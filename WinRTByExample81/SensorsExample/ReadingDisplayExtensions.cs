using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public static class ReadingDisplayExtensions
    {
        public static String DisplayText([NotNull] this CompassReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("Mag: {0} True: {1}", 
                reading.HeadingMagneticNorth, 
                reading.HeadingTrueNorth);
        }

        public static String DisplayText([NotNull] this LightSensorReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("Illuminance(Lux): {0}", 
                reading.IlluminanceInLux);
        }     
        
        public static String DisplayText([NotNull] this AccelerometerReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("X= {0} Y={1} Z={2}",
                reading.AccelerationX,
                reading.AccelerationY,
                reading.AccelerationZ);
        }

        public static String DisplayText([NotNull] this GyrometerReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("X= {0} Y={1} Z={2}", 
                reading.AngularVelocityX, 
                reading.AngularVelocityY,
                reading.AngularVelocityZ);
        }
        
        public static String DisplayText([NotNull] this InclinometerReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("Pitch={0} Roll={1} Yaw={2} Yaw Accuracy={3}",
                reading.PitchDegrees,
                reading.RollDegrees,
                reading.YawDegrees,
                reading.YawAccuracy);
        }

        public static String DisplayText([NotNull] this OrientationSensorReading reading)
        {
            if (reading == null) throw new ArgumentNullException("reading");
            return String.Format("Q(x)= {0} Q(y)={1} Q(z)={2} Q(w)={3}", 
                reading.Quaternion.X,
                reading.Quaternion.Y, 
                reading.Quaternion.Z, 
                reading.Quaternion.W);
        }

        public static String DisplayText(this BasicGeoposition reading)
        {
            return String.Format("Lat= {0} Lon={1} Alt={2}",
                reading.Latitude,
                reading.Longitude,
                reading.Altitude);
        }
    }
}