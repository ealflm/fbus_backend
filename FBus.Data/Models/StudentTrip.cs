using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class StudentTrip
    {
        public Guid StudentTripId { get; set; }
        public Guid StudentId { get; set; }
        public Guid TripId { get; set; }
        public Guid StationId { get; set; }
        public string CopyOfRoute { get; set; }
        public decimal? Rate { get; set; }
        public string Feedback { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool Type { get; set; }
        public int Status { get; set; }

        public virtual Station Station { get; set; }
        public virtual Student Student { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
