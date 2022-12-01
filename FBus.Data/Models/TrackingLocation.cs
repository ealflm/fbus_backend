using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class TrackingLocation
    {
        public Guid TrackingLocationId { get; set; }
        public Guid DriverId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Driver Driver { get; set; }
    }
}
