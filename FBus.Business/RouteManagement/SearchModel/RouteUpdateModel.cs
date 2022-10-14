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
        public List<Guid> AddStationList { get; set; }
        public List<Guid> RemoveStationList { get; set; }
        public int? Status { get; set; }
    }
}
