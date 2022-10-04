using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Station
    {
        public Station()
        {
            RouteStations = new HashSet<RouteStation>();
        }

        public Guid StationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Logitude { get; set; }
        public decimal Latidude { get; set; }
        public int Status { get; set; }

        public virtual ICollection<RouteStation> RouteStations { get; set; }
    }
}
