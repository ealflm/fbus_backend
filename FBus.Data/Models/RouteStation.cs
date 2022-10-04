using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class RouteStation
    {
        public Guid RouteId { get; set; }
        public Guid StationId { get; set; }
        public int OrderNumber { get; set; }
        public decimal? Distance { get; set; }

        public virtual Route Route { get; set; }
        public virtual Station Station { get; set; }
    }
}
