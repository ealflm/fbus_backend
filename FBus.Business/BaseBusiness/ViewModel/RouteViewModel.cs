using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class RouteViewModel
    {
        public Guid RouteId { get; set; }
        public string Name { get; set; }
        public decimal Distance { get; set; }
        public int TotalStation { get; set; }
        public int EstimatedTime { get; set; }
        public List<StationViewModel> StationList { get; set; }
        public int Status { get; set; }
    }
}
