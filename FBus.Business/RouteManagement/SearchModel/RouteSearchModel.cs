using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.RouteManagement.SearchModel
{
    public class RouteSearchModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Distance { get; set; }
        [Required]
        public int TotalStation { get; set; }
        public int? EstimatedTime { get; set; }
        [Required]
        public List<Guid> StationList { get; set; }
        [Required]
        public List<decimal> DistanceList { get; set; }
    }
}
