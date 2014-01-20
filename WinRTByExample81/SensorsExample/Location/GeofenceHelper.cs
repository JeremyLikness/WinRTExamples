using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

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

        public Geofence AddGeofence(
            String fenceId, 
            BasicGeoposition fenceCenter, 
            Double radiusInMeters)
        {
            if (String.IsNullOrWhiteSpace(fenceId)) 
                throw new ArgumentException("A fence id is required.", "fenceId");
            if (fenceId.Length > 64) 
                throw new ArgumentException("The fence id must be <= 64 chars.", "fenceId");

            var fenceCircle = new Geocircle(fenceCenter, radiusInMeters);

            // Default (omitted in ctor) = Entered/Exited
            const MonitoredGeofenceStates states =
                MonitoredGeofenceStates.Entered |
                MonitoredGeofenceStates.Exited |
                MonitoredGeofenceStates.Removed;

            // Create the fence with the desired states and not single-use
            var fence = new Geofence(fenceId, fenceCircle, states, false);
            GeofenceMonitor.Current.Geofences.Add(fence);
            return fence;

            // MORE EXPLICIT GEOFENCE OPTIONS
            // Default (omitted in ctor) = false
            // var isSingleUse = false;

            // Default (omitted in ctor) = 10 seconds
            // var dwellingTime = TimeSpan.FromSeconds(10);

            // Default (omitted in ctor) = immediate
            // var monitoringStartTime = DateTimeOffset.Now;

            // Default (omitted in ctor) = 0 seconds/indefinite
            // var monitoringDuration = TimeSpan.Zero;

            //var fence = new Geofence(fenceId, fenceCircle, states, isSingleUse, dwellingTime, monitoringStartTime, monitoringDuration);
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

        private void HandleGeofenceStateChanged(GeofenceMonitor monitor, Object o)
        {
            // Iterate over and process the accumulated reports
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
                            Position = report.Geoposition.Coordinate.Point.Position,
                            Timestamp = report.Geoposition.Coordinate.Timestamp
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