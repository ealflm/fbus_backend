using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Shift
    {
        public Guid ShiftId { get; set; }
        public Guid DriverId { get; set; }
        public Guid TripId { get; set; }
        public DateTime RequestTime { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
