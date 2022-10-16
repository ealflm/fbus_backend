using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.SearchModel
{
    public class StudentTripSearchModel
    {
        [Required]
        public Guid StudentId { get; set; }
        [Required]
        public Guid TripId { get; set; }
        [Required]
        public Guid StationId { get; set; }
        [Required]
        public bool Type { get; set; }
    }
}
