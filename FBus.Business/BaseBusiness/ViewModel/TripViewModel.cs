using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class TripViewModel
    {
        public Guid TripId { get; set; }
        public BusViewModel Bus { get; set; }
        public DriverViewModel Driver { get; set; }
        public RouteViewModel Route { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public int Status { get; set; }
        public float? Rate { get; set; }
        public int CurrentTicket { get; set; }   
    }
}
