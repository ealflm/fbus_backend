﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class StationViewModel
    {
        public Guid StationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latidude { get; set; }
        public int Status { get; set; }
    }
}
