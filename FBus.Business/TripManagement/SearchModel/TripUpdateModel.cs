using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.TripManagement.SearchModel
{
    public class TripUpdateModel
    {
        public Guid BusId { get; set; }
        public Guid DriverId { get; set; }
        public Guid RouteId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public int Status { get; set; }
    }
}
