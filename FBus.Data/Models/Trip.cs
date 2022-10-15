using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Trip
    {
        public Trip()
        {
            StudentTrips = new HashSet<StudentTrip>();
        }

        public Guid TripId { get; set; }
        public Guid BusId { get; set; }
        public Guid DriverId { get; set; }
        public Guid RouteId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public int Status { get; set; }

        public virtual BusVehicle Bus { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Route Route { get; set; }
        public virtual ICollection<StudentTrip> StudentTrips { get; set; }
    }
}
