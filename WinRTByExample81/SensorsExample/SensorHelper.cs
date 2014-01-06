using System;
using Windows.Devices.Sensors;
using SensorsExample.Annotations;

namespace SensorsExample
{
    /// <summary>
    /// Helper class for sensor access
    /// </summary>
    public class SensorHelper
    {
        #region Fields

        private readonly SensorSettings _sensorSettings;

        private SimpleOrientationSensor _simpleOrientation;
        private Compass _compass;
        private LightSensor _lightSensor;
        private Accelerometer _accelerometer;
        private Gyrometer _gyrometer;
        private Inclinometer _inclinometer;
        private OrientationSensor _orientationSensor; 

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorHelper" /> class.
        /// </summary>
        /// <param name="sensorSettings">The sensor settings.</param>
        public SensorHelper([NotNull] SensorSettings sensorSettings)
        {
            if (sensorSettings == null) throw new ArgumentNullException("sensorSettings");

            _sensorSettings = sensorSettings;

            ConfigureSimpleOrientation();
            ConfigureCompass();
            ConfigureLightSensor();
            ConfigureAccelerometer();
            ConfigureGyrometer();
            ConfigureInclinometer();
            ConfigureOrientationSensor();
        }

        #endregion

        #region Simple Orientation

        private void ConfigureSimpleOrientation()
        {
            _simpleOrientation = SimpleOrientationSensor.GetDefault();
            if (_simpleOrientation != null)
            {
                _sensorSettings.IsSimpleOrientationAvailable = true;

                // NOTE - Simple Orientation does not offer a minimum interval setting
                _simpleOrientation.OrientationChanged += SimpleOrientationOnOrientationChanged;

                // Read the initial sensor value
                _sensorSettings.LatestSimpleOrientationReading = GetSimpleOrientationReaading();
            }
        }

        public SimpleOrientation GetSimpleOrientationReaading()
        {
            if (_simpleOrientation == null) throw new InvalidOperationException("The simple orientation sensor is either not present or has not been initialized");

            var orientation = _simpleOrientation.GetCurrentOrientation();
            return orientation;
            // Available reading values include:
            //args.Orientation
            //args.Timestamp
        }

        private void SimpleOrientationOnOrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            _sensorSettings.LatestSimpleOrientationReading = args.Orientation;
        }

        #endregion

        #region Compass

        private void ConfigureCompass()
        {
            _compass = Compass.GetDefault();
            if (_compass != null)   // Null if not present/supported
            {
                _sensorSettings.IsCompassAvailable = true;

                var minInterval = _compass.MinimumReportInterval;
                _compass.ReportInterval = Math.Max(_sensorSettings.CompassReportInterval, minInterval);
                _compass.ReadingChanged += CompassOnReadingChanged;

                // Read the initial sensor value
                _sensorSettings.LatestCompassReading = GetCompassReading();
            }
        }

        public CompassReading GetCompassReading()
        {
            if (_compass == null) throw new InvalidOperationException("The compass is either not present or has not been initialized");

            var reading = _compass.GetCurrentReading();
            return reading;
            // Available reading values include:
            // reading.HeadingMagneticNorth
            // reading.HeadingTrueNorth
            // reading.Timestamp

            // NOTE: 
            // reading.HeadingAccuracy is a MagnetometerAccracyEnum value
            //  Unknown - app should decide
            //  Unreliable - High inaccuracy...apps should ask for a calibration
            //  Approximate - Good for some, not for others...app should decide
            //  High - All set.
            //  However, you should not prompt the user to calibrate too frequently. We recommend no more than once every 10 minutes.
        }

        private void CompassOnReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            _sensorSettings.LatestCompassReading = args.Reading;
        }

        #endregion

        #region Light Sensor

        private void ConfigureLightSensor()
        {
            _lightSensor = LightSensor.GetDefault();
            if (_lightSensor != null)   // Null if not present/supported
            {
                _sensorSettings.IsLightSensorAvailable = true;

                var minInterval = _lightSensor.MinimumReportInterval;
                _lightSensor.ReportInterval = Math.Max(_sensorSettings.LightSensorReportInterval, minInterval);
                _lightSensor.ReadingChanged += LightSensorOnReadingChanged;

                // Read the initial sensor value
                _sensorSettings.LatestLightSensorReading = GetLightSensorReading();
            }
        }

        public LightSensorReading GetLightSensorReading()
        {
            if (_lightSensor == null) throw new InvalidOperationException("The light sensor is either not present or has not been initialized");

            var reading = _lightSensor.GetCurrentReading();
            return reading;
            // Available reading values include:
            // reading.IlluminanceInLux
            // reading.Timestamp
        }

        private void LightSensorOnReadingChanged(LightSensor sender, LightSensorReadingChangedEventArgs args)
        {
            _sensorSettings.LatestLightSensorReading = args.Reading;
        }

        #endregion

        #region Accelerometer

        private void ConfigureAccelerometer()
        {
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)   // Null if not present/supported
            {
                _sensorSettings.IsAccelerometerAvailable = true;

                var minInterval = _accelerometer.MinimumReportInterval;
                _accelerometer.ReportInterval = Math.Max(_sensorSettings.AccelerometerReportInterval, minInterval);
                _accelerometer.ReadingChanged += AccelerometerOnReadingChanged;
                _accelerometer.Shaken += AccelerometerOnShaken;

                // Read the initial sensor value
                _sensorSettings.LatestAccelerometerReading = GetAccelerometerReading();
            }
        }

        public AccelerometerReading GetAccelerometerReading()
        {
            if (_accelerometer == null) throw new InvalidOperationException("The accelerometer is either not present or has not been initialized");

            var reading = _accelerometer.GetCurrentReading();
            return reading;
            // Available reading values include:
            // reading.AccelerationX
            // reading.AccelerationY
            // reading.AccelerationZ
            // reading.Timestamp
        }

        private void AccelerometerOnReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            _sensorSettings.LatestAccelerometerReading = args.Reading;
        }

        private void AccelerometerOnShaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            // args.Timestamp
        }

        #endregion

        #region Gyrometer

        private void ConfigureGyrometer()
        {
            _gyrometer = Gyrometer.GetDefault();
            if (_gyrometer != null)   // Null if not present/supported
            {
                _sensorSettings.IsGyrometerAvailable = true;

                var minInterval = _gyrometer.MinimumReportInterval;
                _gyrometer.ReportInterval = Math.Max(_sensorSettings.GyrometerReportInterval, minInterval);
                _gyrometer.ReadingChanged += GyrometerOnReadingChanged;

                // Read the initial sensor value
                _sensorSettings.LatestGyrometerReading = GetGyrometerReading();
            }
        }

        public GyrometerReading GetGyrometerReading()
        {
            if (_gyrometer == null) throw new InvalidOperationException("The gyrometer is either not present or has not been initialized");

            var reading = _gyrometer.GetCurrentReading();
            return reading;
            // Available reading values include:
            //reading.AngularVelocityX
            //reading.AngularVelocityY
            //reading.AngularVelocityZ
            //reading.Timestamp
        }

        private void GyrometerOnReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            //var reading = args.Reading;
            _sensorSettings.LatestGyrometerReading = args.Reading;
        }

        #endregion

        #region Inclinometer

        private void ConfigureInclinometer()
        {
            _inclinometer = Inclinometer.GetDefault();
            if (_inclinometer != null)   // Null if not present/supported
            {
                _sensorSettings.IsInclinometerAvailable = true;

                var minInterval = _inclinometer.MinimumReportInterval;
                _inclinometer.ReportInterval = Math.Max(_sensorSettings.InclinometerReportInterval, minInterval);
                _inclinometer.ReadingChanged += InclinometerOnReadingChanged;

                // Read the initial sensor value
                _sensorSettings.LatestInclinometerReading = GetInclinometerReading();
            }
        }

        public InclinometerReading GetInclinometerReading()
        {
            if (_inclinometer == null) throw new InvalidOperationException("The inclinometer is either not present or has not been initialized");

            var reading = _inclinometer.GetCurrentReading();
            return reading;
            // Available reading values include:
            // reading.PitchDegrees
            // reading.RollDegrees
            // reading.YawDegrees
            // reading.YawAccuracy
            // reading.Timestamp
        }

        private void InclinometerOnReadingChanged(Inclinometer sender, InclinometerReadingChangedEventArgs args)
        {
            _sensorSettings.LatestInclinometerReading = args.Reading;
        }

        #endregion

        #region Orientation Sensor

        private void ConfigureOrientationSensor()
        {
            _orientationSensor = OrientationSensor.GetDefault();
            if (_orientationSensor != null)   // Null if not present/supported
            {
                _sensorSettings.IsOrientationSensorAvailable = true;

                var minInterval = _orientationSensor.MinimumReportInterval;
                _orientationSensor.ReportInterval = Math.Max(_sensorSettings.OrientationSensorReportInterval, minInterval);
                _orientationSensor.ReadingChanged += OrientationSensorOnReadingChanged;

                // Read the initial sensor value
                _sensorSettings.LatestOrientationSensorReading = GetOrientationSensorReading();
            }
        }

        public OrientationSensorReading GetOrientationSensorReading()
        {
            if (_orientationSensor == null) throw new InvalidOperationException("The orientation sensor is either not present or has not been initialized");

            var reading = _orientationSensor.GetCurrentReading();
            return reading;
            // Available reading values include:
            // reading.Quaternion
            // reading.RotationMatrix
            // reading.YawAccuracy
            // reading.Timestamp
        }

        private void OrientationSensorOnReadingChanged(OrientationSensor sender, OrientationSensorReadingChangedEventArgs args)
        {
            _sensorSettings.LatestOrientationSensorReading = args.Reading;
        } 

        #endregion
    }
}