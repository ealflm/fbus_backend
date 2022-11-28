using FBus.Business.BaseBusiness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BusVehicleManagement.Models
{
    public class BusVehicleDetailModel
    {
        public BusViewModel BusVehicle { get; set; }
        public List<TripViewModel> Trips { get; set; }
    }
}
