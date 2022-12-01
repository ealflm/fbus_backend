using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.Student.Models
{
    public class StudentStatisticsModel
    {
        public int StudentTripCount { get; set; }
        public int StudentTripNotUseCount { get; set; }
        public double Distance { get; set; }
    }
}
