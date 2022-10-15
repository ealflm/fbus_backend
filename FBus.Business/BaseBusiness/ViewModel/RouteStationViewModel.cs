using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class RouteStationViewModel
    {
        public Guid RouteId { get; set; }
        public Guid StationId { get; set; }
        public int OrderNumber { get; set; }
        public decimal? Distance { get; set; }
    }
}
