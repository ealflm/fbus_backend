using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBus.Business.Utils.CustomAnnotation;

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
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateEnd { get; set; }
        [Required]
        [TimeSpanComparator("TimeEnd")]
        public string TimeStart { get; set; }
        [Required]
        public string TimeEnd { get; set; }
    }
}
