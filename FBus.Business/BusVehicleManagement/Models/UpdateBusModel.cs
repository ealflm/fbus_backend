namespace FBus.Business.BusVehicleManagement.Models
{
    public class UpdateBusModel
    {
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int? Seat { get; set; }
        public int? Status { get; set; }
    }
}