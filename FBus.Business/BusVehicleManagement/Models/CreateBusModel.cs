using System.ComponentModel.DataAnnotations;

namespace FBus.Business.BusVehicleManagement.Models
{
    public class CreateBusModel
    {
        [Required]
        public string LicensePlates { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public int Seat { get; set; }
    }
}