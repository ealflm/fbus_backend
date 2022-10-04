using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Bu
    {
        public Bu()
        {
            Trips = new HashSet<Trip>();
        }

        public Guid BusId { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int Seat { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Trip> Trips { get; set; }
    }
}
