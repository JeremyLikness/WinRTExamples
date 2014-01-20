using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using SensorsExample.Annotations;
using SensorsExample.Common;

namespace SensorsExample
{
    public class GeofenceItemViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly GeofenceHelper _geofenceHelper;
        private readonly BasicGeoposition _fenceCenter;
        private const Double FenceRadiusMiles = 20.0;

        private String _geofenceName;
        private RelayCommand _addFenceCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GeofenceItemViewModel" /> class.
        /// </summary>
        /// <param name="geofenceHelper">The geofence helper.</param>
        /// <param name="fenceCenter">The fence center.</param>
        /// <exception cref="System.ArgumentNullException">geofenceHelper</exception>
        public GeofenceItemViewModel([NotNull] GeofenceHelper geofenceHelper, BasicGeoposition fenceCenter)
        {
            if (geofenceHelper == null) throw new ArgumentNullException("geofenceHelper");

            _geofenceHelper = geofenceHelper;
            _fenceCenter = fenceCenter;
        } 

        #endregion

        public String GeofenceName
        {
            get { return _geofenceName; }
            set
            {
                if (value == _geofenceName) return;
                _geofenceName = value;
                _addFenceCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public Double Latitude
        {
            get { return _fenceCenter.Latitude; }
        }

        public Double Longitude
        {
            get { return _fenceCenter.Longitude; }
        }

        public Double RadiusMiles
        {
            get { return FenceRadiusMiles; }
        }

        public ICommand AddFenceCommand
        {
            get { return _addFenceCommand ?? (_addFenceCommand = new RelayCommand(AddFence, CanAddFence)); }
        }

        private void AddFence()
        {
            if (String.IsNullOrWhiteSpace(GeofenceName)) throw new InvalidOperationException("A geofence name is required.");
            var geofence = _geofenceHelper.AddGeofence(GeofenceName, _fenceCenter, RadiusMiles);
            if (geofence != null)
            {
                OnFenceAdded(geofence);    
            }
        }

        private Boolean CanAddFence()
        {
            return !String.IsNullOrWhiteSpace(GeofenceName);
        }

        public event EventHandler<GeofenceAddedEventArgs> FenceAdded;

        private void OnFenceAdded(Geofence geofence)
        { 
            var handler = FenceAdded;
            if (handler != null) handler(this, new GeofenceAddedEventArgs {Geofence = geofence});
        }

        #region INotifyPropertyChanged Implementation
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
 
        #endregion    
    }

    public class GeofenceAddedEventArgs : EventArgs
    {
        public Geofence Geofence { get; set; }
    }

    internal class SampleGeofenceItemViewModel : GeofenceItemViewModel
    {
        public SampleGeofenceItemViewModel()
            : base(new GeofenceHelper(), new BasicGeoposition {Latitude = 42.733166, Longitude = -71.502839})

        {
        }
    }

}