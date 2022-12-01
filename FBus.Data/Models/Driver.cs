using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Driver
    {
        public Driver()
        {
            DriverNotifications = new HashSet<DriverNotification>();
            Shifts = new HashSet<Shift>();
            TrackingLocations = new HashSet<TrackingLocation>();
            Trips = new HashSet<Trip>();
        }

        public Guid DriverId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string PhotoUrl { get; set; }
        public string Address { get; set; }
        public string NotifyToken { get; set; }
        public int Status { get; set; }

        public virtual ICollection<DriverNotification> DriverNotifications { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<TrackingLocation> TrackingLocations { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
