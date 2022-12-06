using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Route
    {
        public Route()
        {
            FavoriteRoutes = new HashSet<FavoriteRoute>();
            RouteStations = new HashSet<RouteStation>();
            Trips = new HashSet<Trip>();
        }

        public Guid RouteId { get; set; }
        public string Name { get; set; }
        public decimal Distance { get; set; }
        public int TotalStation { get; set; }
        public int? EstimatedTime { get; set; }
        public int Status { get; set; }

        public virtual ICollection<FavoriteRoute> FavoriteRoutes { get; set; }
        public virtual ICollection<RouteStation> RouteStations { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
