using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public class SensorSettings : INotifyPropertyChanged
    {
        #region Fields

        private readonly SynchronizationContext _context = SynchronizationContext.Current;

        private DisplayOrientations _displayOrientation;
        private Boolean _compensateForDisplayOrientation = true;
        private UInt32 _sensorReportInterval = 60;

        private Boolean _isSimpleOrientationAvailable;
        private SimpleOrientation _latestSimpleOrientationReading;

        private Boolean _isCompassAvailable;
        private Boolean _isFollowingCompass;
        private CompassReading _latestCompassReading;

        private Boolean _isLightSensorAvailable;
        private LightSensorReading _latestLightSensorReading;

        private Boolean _isAccelerometerAvailable;
        private AccelerometerReading _latestAccelerometerReading;

        private Boolean _isGyrometerAvailable;
        private GyrometerReading _latestGyrometerReading;

        private Boolean _isInclinometerAvailable;
        private Boolean _followInclinometer;
        private InclinometerReading _latestInclinometerReading;

        private Boolean _isLocationAvailable;
        private Boolean _isLocationRequestingHighAccuracy;
        private Geocoordinate _latestLocationReading;

        private Boolean _isOrientationSensorAvailable;
        private OrientationSensorReading _latestOrientationSensorReading;
        private DateTimeOffset _lastAccelerometerShakeTime;

        #endregion

        #region Display Orientation

        public DisplayOrientations DisplayOrientation
        {
            get { return _displayOrientation; }
            set
            {
                if (value == _displayOrientation) return;
                _displayOrientation = value;
                OnPropertyChanged();
            }
        }

        public Boolean CompensateForDisplayOrientation
        {
            get { return _compensateForDisplayOrientation; }
            set
            {
                if (value.Equals(_compensateForDisplayOrientation)) return;
                _compensateForDisplayOrientation = value;
                OnPropertyChanged();
            }
        } 
        
        #endregion

        #region ReportIntervals

        public UInt32 SensorReportInterval
        {
            get { return _sensorReportInterval; }
            set
            {
                if (value == _sensorReportInterval) return;
                _sensorReportInterval = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Simple Orientation

        public Boolean IsSimpleOrientationAvailable
        {
            get { return _isSimpleOrientationAvailable; }
            set
            {
                if (value.Equals(_isSimpleOrientationAvailable)) return;
                _isSimpleOrientationAvailable = value;
                OnPropertyChanged();
            }
        }

        public SimpleOrientation LatestSimpleOrientationReading
        {
            get { return _latestSimpleOrientationReading; }
            set
            {
                if (value == _latestSimpleOrientationReading) return;
                _latestSimpleOrientationReading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Compass

        public Boolean IsCompassAvailable
        {
            get { return _isCompassAvailable; }
            set
            {
                if (value.Equals(_isCompassAvailable)) return;
                _isCompassAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsFollowingCompass
        {
            get { return _isFollowingCompass; }
            set
            {
                if (value.Equals(_isFollowingCompass)) return;
                _isFollowingCompass = value;
                OnPropertyChanged();
            }
        }

        public CompassReading LatestCompassReading
        {
            get { return _latestCompassReading; }
            set
            {
                if (Equals(value, _latestCompassReading)) return;
                _latestCompassReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestCompassReadingText");
            }
        }

        public String LatestCompassReadingText
        {
            get { return GetCompassReadingDisplayText(LatestCompassReading); }
        }

        #endregion

        #region Inclinometer

        public Boolean IsInclinometerAvailable
        {
            get { return _isInclinometerAvailable; }
            set
            {
                if (value.Equals(_isInclinometerAvailable)) return;
                _isInclinometerAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean FollowInclinometer
        {
            get { return _followInclinometer; }
            set
            {
                if (value.Equals(_followInclinometer)) return;
                _followInclinometer = value;
                OnPropertyChanged();
            }
        }

        public InclinometerReading LatestInclinometerReading
        {
            get { return _latestInclinometerReading; }
            set
            {
                if (Equals(value, _latestInclinometerReading)) return;
                _latestInclinometerReading = value;
                OnPropertyChanged();
                OnPropertyChanged("InclinometerReadingText");
            }
        }

        public String InclinometerReadingText
        {
            get { return GetInclinometerReadingDisplayText(LatestInclinometerReading); }
        }

        #endregion
        
        #region Accelerometer

        public Boolean IsAccelerometerAvailable
        {
            get { return _isAccelerometerAvailable; }
            set
            {
                if (value.Equals(_isAccelerometerAvailable)) return;
                _isAccelerometerAvailable = value;
                OnPropertyChanged();
            }
        }

        public AccelerometerReading LatestAccelerometerReading
        {
            get { return _latestAccelerometerReading; }
            set
            {
                if (value == _latestAccelerometerReading) return;
                _latestAccelerometerReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestAccelerometerReadingText");
            }
        }

        public String LatestAccelerometerReadingText
        {
            get { return GetAccelerometerReadingDisplayText(LatestAccelerometerReading); }
        }

        public DateTimeOffset LatestAccelerometerShakeTime
        {
            get { return _lastAccelerometerShakeTime; }
            set
            {
                if (value.Equals(_lastAccelerometerShakeTime)) return;
                _lastAccelerometerShakeTime = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestAccelerometerShakeTimeText");
            }
        }

        public String LatestAccelerometerShakeTimeText
        {
            get
            {
                return LatestAccelerometerShakeTime == DateTimeOffset.MinValue
                    ? String.Format("No shake detected")
                    : String.Format("Shake detected: {0:h:mm:ss tt}", LatestAccelerometerShakeTime); 
            }
        }

        #endregion

        #region Gyrometer

        public Boolean IsGyrometerAvailable
        {
            get { return _isGyrometerAvailable; }
            set
            {
                if (value.Equals(_isGyrometerAvailable)) return;
                _isGyrometerAvailable = value;
                OnPropertyChanged();
            }
        }

        public GyrometerReading LatestGyrometerReading
        {
            get { return _latestGyrometerReading; }
            set
            {
                if (value == _latestGyrometerReading) return;
                _latestGyrometerReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestGyrometerReadingText");
            }
        }

        public String LatestGyrometerReadingText
        {
            get { return GetGyrometerReadingDisplayText(LatestGyrometerReading); }
        }

        #endregion

        #region Light Sensor

        public Boolean IsLightSensorAvailable
        {
            get { return _isLightSensorAvailable; }
            set
            {
                if (value.Equals(_isLightSensorAvailable)) return;
                _isLightSensorAvailable = value;
                OnPropertyChanged();
            }
        }

        public LightSensorReading LatestLightSensorReading
        {
            get { return _latestLightSensorReading; }
            set
            {
                if (value == _latestLightSensorReading) return;
                _latestLightSensorReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestLightSensorReadingText");
            }
        }

        public String LatestLightSensorReadingText
        {
            get
            {
                if (LatestLightSensorReading == null) return "No Reading Available.";
                return GetLightSensorDisplayText(LatestLightSensorReading);
            }
        }

        #endregion

        #region Orientation Sensor

        public Boolean IsOrientationSensorAvailable
        {
            get { return _isOrientationSensorAvailable; }
            set
            {
                if (value.Equals(_isOrientationSensorAvailable)) return;
                _isOrientationSensorAvailable = value;
                OnPropertyChanged();
            }
        }

        public OrientationSensorReading LatestOrientationSensorReading
        {
            get { return _latestOrientationSensorReading; }
            set
            {
                if (value == _latestOrientationSensorReading) return;
                _latestOrientationSensorReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestOrientationSensorReadingText");
            }
        }

        public String LatestOrientationSensorReadingText
        {
            get
            {
                if (LatestOrientationSensorReading == null) return "No Reading Available.";
                return GetOrientationSensorDisplayText(LatestOrientationSensorReading);
            }
        }

        #endregion

        #region Location

        public Boolean IsLocationAvailable
        {
            get { return _isLocationAvailable; }
            set
            {
                if (value.Equals(_isLocationAvailable)) return;
                _isLocationAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsLocationRequestingHighAccuracy
        {
            get { return _isLocationRequestingHighAccuracy; }
            set
            {
                if (value.Equals(_isLocationRequestingHighAccuracy)) return;
                _isLocationRequestingHighAccuracy = value;
                OnPropertyChanged();
            }
        }

        public Geocoordinate LatestLocationReading
        {
            get { return _latestLocationReading; }
            set
            {
                if (value.Equals(_latestLocationReading)) return;
                _latestLocationReading = value;
                OnPropertyChanged();
                OnPropertyChanged("LatestLocationReadingText");
            }
        }

        public String LatestLocationReadingText
        {
            get
            {
                return LatestLocationReading == null
                    ? "No Reading"
                    : LatestLocationReading.Point.Position.DisplayText(true);
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                // Use the sync context to martial the raising of the property change notification, 
                // since some of these changes are triggered by sensor events that occur off of the UI thread.
                _context.Post(x => handler(this, new PropertyChangedEventArgs(propertyName)), null);
            }
        } 

        #endregion

        #region Display Text calculations

        public String GetCompassReadingDisplayText(CompassReading reading)
        {
            if (reading == null) return "No Reading Available.";
            
            var offset = 0;
            if (CompensateForDisplayOrientation)
            {
                offset = DisplayOrientation.CompassOffset();
            }

            return
                String.Format("Mag: {0} True: {1}",
                    (LatestCompassReading.HeadingMagneticNorth + offset)%360,
                    LatestCompassReading.HeadingTrueNorth.HasValue
                        ? ((LatestCompassReading.HeadingTrueNorth + offset)%360).ToString()
                        : "Not Available");
        }

        public String GetInclinometerReadingDisplayText(InclinometerReading reading)
        {
            if (reading == null) return "No Reading Available.";

            var axisAdjustment = SensorExtensions.AxisOffset.Default;
            if (CompensateForDisplayOrientation)
            {
                axisAdjustment = DisplayOrientation.AxisAdjustmentFactor();
            }

            var adjustedPitchDegrees = reading.PitchDegrees*axisAdjustment.X;
            var adjustedRollDegrees = reading.RollDegrees*axisAdjustment.Y;
            var adjustedYawDegrees = reading.YawDegrees*axisAdjustment.Z;
        
            return String.Format("Pitch={0} Roll={1} Yaw={2}",
                adjustedPitchDegrees,
                adjustedRollDegrees,
                adjustedYawDegrees);
        }

        public String GetAccelerometerReadingDisplayText(AccelerometerReading reading)
        {
            if (reading == null) return "No Reading Available.";

            var axisAdjustment = SensorExtensions.AxisOffset.Default;
            if (CompensateForDisplayOrientation)
            {
                axisAdjustment = DisplayOrientation.AxisAdjustmentFactor();
            }

            var adjustedAccelerationX = reading.AccelerationX * axisAdjustment.X;
            var adjustedAccelerationY = reading.AccelerationY * axisAdjustment.Y;
            var adjustedAccelerationZ = reading.AccelerationZ * axisAdjustment.Z;

            return String.Format("X= {0} Y={1} Z={2}",
                adjustedAccelerationX,
                adjustedAccelerationY,
                adjustedAccelerationZ);
        }

        public String GetGyrometerReadingDisplayText(GyrometerReading reading)
        {
            if (reading == null) return "No Reading Available.";

            var axisAdjustment = SensorExtensions.AxisOffset.Default;
            if (CompensateForDisplayOrientation)
            {
                axisAdjustment = DisplayOrientation.AxisAdjustmentFactor();
            }

            var adjustedAngularVelocityX = reading.AngularVelocityX*axisAdjustment.X;
            var adjustedAngularVelocityY = reading.AngularVelocityY*axisAdjustment.Y;
            var adjustedAngularVelocityZ = reading.AngularVelocityZ*axisAdjustment.Z;

            return String.Format("X= {0} Y={1} Z={2}",
                adjustedAngularVelocityX,
                adjustedAngularVelocityY,
                adjustedAngularVelocityZ);
        }

        public String GetOrientationSensorDisplayText(OrientationSensorReading reading)
        {
            if (reading == null) return "No Reading Available.";

            return String.Format("Q(x)= {0} Q(y)={1} Q(z)={2} Q(w)={3}",
                reading.Quaternion.X,
                reading.Quaternion.Y,
                reading.Quaternion.Z,
                reading.Quaternion.W);
        }

        public String GetLightSensorDisplayText(LightSensorReading reading)
        {
            if (reading == null) return "No Reading Available.";

            return String.Format("Illuminance(Lux): {0}",
                reading.IlluminanceInLux);
        }  

        #endregion
    }
}