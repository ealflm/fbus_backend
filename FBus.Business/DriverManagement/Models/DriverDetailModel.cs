using FBus.Business.BaseBusiness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.DriverManagement.Models
{
    public class DriverDetailModel
    {
        public DriverViewModel Driver { get; set; }
        public List<TripViewModel> Trips { get; set; }
        public float? Rate { get; set; }
        public List<string> Feedback { get; set; }
    }
}
