using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Shift
    {
        public Guid ShiftId { get; set; }
        public Guid DriverId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }

        public virtual Driver Driver { get; set; }
    }
}
