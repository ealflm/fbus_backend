using System;

namespace FBus.Business.BaseBusiness.ViewModel
{
    public class BusViewModel
    {
        public Guid BusId { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int Seat { get; set; }
        public int Status { get; set; }
    }
}