using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.RouteManagement.SearchModel
{
    public class RouteUpdateModel
    {
        public string Name { get; set; }
        public decimal? Distance { get; set; }
        public int? TotalStation { get; set; }
        public int? EstimatedTime { get; set; }
        public List<Guid> StationList { get; set; }
        public List<decimal> DistanceList { get; set; }
        public int? Status { get; set; }
    }
}
