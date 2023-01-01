using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.DriverManagement.Models
{
    public class DriverStatisticsModel
    {
        public int TripCount { get; set; }
        public int TripNotUseCount { get; set; }
        public double Distance { get; set; }
    }
}
