using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class DriverShift
    {
        public DriverShift()
        {
            Trips = new HashSet<Trip>();
        }

        public Guid DriverShiftId { get; set; }
        public Guid ShiftId { get; set; }
        public Guid DriverId { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
