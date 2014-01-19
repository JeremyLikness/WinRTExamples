using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using SensorsExample.Annotations;

namespace SensorsExample
{
    public class GeofenceHelper
    {
        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GeofenceHelper"/> class.
        /// </summary>
        public GeofenceHelper()
        {
            GeofenceMonitor.Current.GeofenceStateChanged += HandleGeofenceStateChanged;
        } 

        #endregion

        public Geofence AddGeofence([NotNull] String fenceId, BasicGeoposition fenceCenter)
        {
            if (String.IsNullOrWhiteSpace(fenceId)) throw new ArgumentException("A fence id is required.", "fenceId");
            if (fenceId.Length > 64) throw new ArgumentException("Fence id must be 64 chars or less.", "fenceId");

            const Double radiusInMeters = 20.0 * 1000; // 20KM
            var fenceCircle = new Geocircle(fenceCenter, radiusInMeters);

            // Default (omitted in ctor) = Entered/Exited
            var states =
                MonitoredGeofenceStates.Entered |
                MonitoredGeofenceStates.Exited |
                MonitoredGeofenceStates.Removed;

            // Default (omitted in ctor) = false
            var isSingleUse = false;
           
            // Default (omitted in ctor) = 10 seconds
            var dwellingTime = TimeSpan.FromSeconds(10);

            // Default (omitted in ctor) = immediate
            var monitoringStartTime = DateTimeOffset.Now;

            // Default (omitted in ctor) = 0 seconds/indefinite
            var monitoringDuration = TimeSpan.FromSeconds(0);

            var fence = new Geofence(fenceId, fenceCircle, states, isSingleUse, dwellingTime, monitoringStartTime, monitoringDuration);
            GeofenceMonitor.Current.Geofences.Add(fence);
            return fence;
            // List of fences
            //monitor.Geofences
            //monitor.LastKnownGeoposition
            //monitor.ReadReports()
            //monitor.Status GeofenceMonitorStatus.Ready//.NotInitialized //.NotAvailable//.NoData//.Initializing// .Disabled, 
            //monitor.StatusChanged += (sender, args) => 
        }

        public void RemoveGeofence(String idToRemove)
        {
            var itemToRemove = GeofenceMonitor.Current.Geofences.FirstOrDefault(x => x.Id.Equals(idToRemove));
            GeofenceMonitor.Current.Geofences.Remove(itemToRemove);
        }

        public IEnumerable<Geofence> GetCurrentFences()
        {
            return GeofenceMonitor.Current.Geofences;
        }

        public event EventHandler<FenceUpdateEventArgs> FenceUpdated;

        private void OnFenceUpdated(FenceUpdateEventArgs args)
        {
            var handler = FenceUpdated;
            if (handler != null) handler(this, args);
        }

        public event EventHandler<FenceRemovedEventArgs> FenceRemoved;

        private void OnFenceRemoved(FenceRemovedEventArgs args)
        {
            var handler = FenceRemoved;
            if (handler != null) handler(this, args);
        }

        private void HandleGeofenceStateChanged(GeofenceMonitor monitor, Object args)
        {
            // Pull a list of all geofence events that have occurred since the last time this was called
            var reports = monitor.ReadReports();
            foreach (var report in reports)
            {
                switch (report.NewState)
                {
                    case GeofenceState.Entered:
                    case GeofenceState.Exited:
                        var updateArgs = new FenceUpdateEventArgs
                        {
                            FenceId = report.Geofence.Id,
                            Reason = report.NewState.ToString(),
                            Timestamp = report.Geoposition.Coordinate.Timestamp,
                            Position = report.Geoposition.Coordinate.Point.Position
                        };
                        OnFenceUpdated(updateArgs);
                        break;
                    case GeofenceState.Removed:
                        var removedArgs = new FenceRemovedEventArgs
                        {
                            FenceId = report.Geofence.Id,
                            WhyRemoved = report.RemovalReason.ToString()
                        };
                        OnFenceRemoved(removedArgs);
                        break;
                }
            }
        }
    }

    public class FenceDescription
    {
        public String FenceId { get; set; }
        public BasicGeoposition CenterPosition { get; set; }
        public Double Radius{ get; set; }
    }

    public class FenceUpdateEventArgs : EventArgs
    {
        public String FenceId { get; set; }
        public String Reason { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public BasicGeoposition Position { get; set; }
    }

    public class FenceRemovedEventArgs : EventArgs
    {
        public String FenceId { get; set; }
        public String WhyRemoved { get; set; }
    }
}