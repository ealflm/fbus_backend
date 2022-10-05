using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StationManagement.SearchModel
{
    public class StationSearchModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public decimal Latidude { get; set; }
    }
}
