using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Shift
    {
        public Shift()
        {
            DriverShifts = new HashSet<DriverShift>();
        }

        public Guid ShiftId { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DriverShift> DriverShifts { get; set; }
    }
}
