using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public class SensorSettings : INotifyPropertyChanged
    {
        #region Fields

        private readonly SynchronizationContext _context = SynchronizationContext.Current;

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
        private BasicGeoposition _latestLocationReading;

        private Boolean _isOrientationSensorAvailable;
        private OrientationSensorReading _latestOrientationSensorReading;

        #endregion

        #region ReportIntervals

        public UInt32 CompassReportInterval { get; set; }
        public UInt32 LightSensorReportInterval { get; set; }
        public UInt32 AccelerometerReportInterval { get; set; }
        public UInt32 GyrometerReportInterval { get; set; }
        public UInt32 InclinometerReportInterval { get; set; }
        public UInt32 OrientationSensorReportInterval { get; set; } 

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
            get
            {
                if (LatestCompassReading == null) return "No Reading Available.";
                return LatestCompassReading.DisplayText();
            }
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
                return LatestLightSensorReading.DisplayText();
            }
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
            get
            {
                if (LatestAccelerometerReading == null) return "No Reading Available.";
                return LatestAccelerometerReading.DisplayText();
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
            get
            {
                if (LatestGyrometerReading == null) return "No Reading Available.";
                return LatestGyrometerReading.DisplayText();
            }
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
            get
            {
                if (LatestInclinometerReading == null) return "No Reading Available.";
                return LatestInclinometerReading.DisplayText();
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

        public BasicGeoposition LatestLocationReading
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
            get { return LatestLocationReading.DisplayText(); }
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
                return LatestOrientationSensorReading.DisplayText();
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
    }
}