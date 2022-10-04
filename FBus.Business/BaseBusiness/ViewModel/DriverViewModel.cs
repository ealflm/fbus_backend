using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class DriverViewModel
    {
        public Guid DriverId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
    }
}
