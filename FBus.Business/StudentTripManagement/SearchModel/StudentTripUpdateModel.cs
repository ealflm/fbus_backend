using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.SearchModel
{
    public class StudentTripUpdateModel
    {
        public Guid TripId { get; set; }
        public Guid StationId { get; set; }
        public bool Type { get; set; }
        public int Status { get; set; }
    }
}
