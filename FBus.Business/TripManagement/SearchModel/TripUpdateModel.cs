using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBus.Business.Utils.CustomAnnotation;

namespace FBus.Business.TripManagement.SearchModel
{
    public class TripUpdateModel
    {
        public Guid BusId { get; set; }

        public Guid? DriverId { get; set; }

        public Guid RouteId { get; set; }

        public DateTime Date { get; set; }

        [TimeSpanComparator("TimeEnd")]
        public string TimeStart { get; set; }

        public string TimeEnd { get; set; }

        public int Status { get; set; }
    }
}
