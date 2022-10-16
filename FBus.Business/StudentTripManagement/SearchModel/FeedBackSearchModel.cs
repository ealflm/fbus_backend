using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.SearchModel
{
    public class FeedBackSearchModel
    {
        [Required]
        public decimal Rate { get; set; }
        public string Feedback { get; set; }
    }
}
