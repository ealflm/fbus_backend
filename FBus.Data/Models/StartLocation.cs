using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class StartLocation
    {
        public Guid StartLocationId { get; set; }
        public Guid StudentId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Address { get; set; }
        public DateTime SearchDate { get; set; }

        public virtual Student Student { get; set; }
    }
}
