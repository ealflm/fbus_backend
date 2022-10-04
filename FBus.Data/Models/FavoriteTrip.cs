using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class FavoriteTrip
    {
        public Guid RouteId { get; set; }
        public Guid StudentId { get; set; }
        public TimeSpan Time { get; set; }
        public string Name { get; set; }
        public string Desciption { get; set; }

        public virtual Route Route { get; set; }
        public virtual Student Student { get; set; }
    }
}
