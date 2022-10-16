using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class StudentTripViewModel
    {
        public Guid StudentTripId { get; set; }
        public StudentViewModel Student { get; set; }
        public TripViewModel Trip { get; set; }
        public StationViewModel Station { get; set; }
        public string CopyOfRoute { get; set; }
        public decimal? Rate { get; set; }
        public string Feedback { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool Type { get; set; }
        public int Status { get; set; }
    }
}
