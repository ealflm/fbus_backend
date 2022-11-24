using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.TripManagement.SearchModel
{
    public class TripSearchModel
    {
        [Required]
        public Guid BusId { get; set; }
        [Required]
        public Guid DriverId { get; set; }
        [Required]
        public Guid RouteId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string TimeStart { get; set; }
        [Required]
        public string TimeEnd { get; set; }
    }
}
