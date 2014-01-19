using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using SensorsExample.Annotations;
using SensorsExample.Common;

namespace SensorsExample
{
    public class GeofenceListingViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly GeofenceHelper _geofenceHelper;
        private readonly ObservableCollection<Geofence> _currentGeofences = new ObservableCollection<Geofence>();
        private Geofence _selectedGeofence;
        private RelayCommand _removeSelectedItemCommand;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GeofenceListingViewModel" /> class.
        /// </summary>
        /// <param name="geofenceHelper">The geofence helper.</param>
        /// <exception cref="System.ArgumentNullException">geofenceHelper</exception>
        public GeofenceListingViewModel([NotNull] GeofenceHelper geofenceHelper)
        {
            if (geofenceHelper == null) throw new ArgumentNullException("geofenceHelper");

            _geofenceHelper = geofenceHelper;


            if (!DesignMode.DesignModeEnabled)
            {
                _currentGeofences = new ObservableCollection<Geofence>(_geofenceHelper.GetCurrentFences());
            }
        } 

        #endregion

        public IEnumerable<Geofence> CurrentGeofences
        {
            get { return _currentGeofences; }
        }

        protected ObservableCollection<Geofence> CurrentGeofencesCollection
        {
            get { return _currentGeofences; }
        }

        public Geofence SelectedGeofence
        {
            get { return _selectedGeofence; }
            set
            {
                if (Equals(value, _selectedGeofence)) return;
                _selectedGeofence = value;
                _removeSelectedItemCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public ICommand RemoveSelectedItemCommand
        {
            get { return _removeSelectedItemCommand ?? (_removeSelectedItemCommand = new RelayCommand(RemoveSelectedItem, CanRemoveSelectedItem)); }
        }

        public event EventHandler<GeofenceRemovedEventArgs> FenceRemoved;

        protected virtual void OnFenceRemoved(Geofence fence)
        {
            var handler = FenceRemoved;
            if (handler != null) handler(this, new GeofenceRemovedEventArgs {Geofence = fence});
        }

        private void RemoveSelectedItem()
        {
            var fenceToRemove = SelectedGeofence;
            _geofenceHelper.RemoveGeofence(fenceToRemove.Id);
            CurrentGeofencesCollection.Remove(fenceToRemove);
            OnFenceRemoved(fenceToRemove);
        }

        private Boolean CanRemoveSelectedItem()
        {
            return SelectedGeofence != null;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion
    }

    public class GeofenceRemovedEventArgs : EventArgs
    {
        public Geofence Geofence { get; set; }
    }

    internal class SampleGeofenceListingViewModel : GeofenceListingViewModel
    {
        public SampleGeofenceListingViewModel()
            : base(new GeofenceHelper())
        {
            if (DesignMode.DesignModeEnabled)
            {
                var geoposition = new BasicGeoposition {Latitude = 42.733166, Longitude = -71.502839};
                var radius = 20000;
                CurrentGeofencesCollection.Add(new Geofence("Sample One", new Geocircle(geoposition, radius)));
                CurrentGeofencesCollection.Add(new Geofence("Sample Two", new Geocircle(geoposition, radius)));
                CurrentGeofencesCollection.Add(new Geofence("Sample Three", new Geocircle(geoposition, radius)));
                CurrentGeofencesCollection.Add(new Geofence("Sample Four", new Geocircle(geoposition, radius)));
                CurrentGeofencesCollection.Add(new Geofence("Sample Five", new Geocircle(geoposition, radius)));
            }
        }
    }
}