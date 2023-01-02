using FBus.Business.BaseBusiness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.TripManagement.SearchModel
{
    public class TripGetStudentTripModel
    {
        public StationViewModel Station { get; set; }
        public int Count { get; set; }
    }
}
