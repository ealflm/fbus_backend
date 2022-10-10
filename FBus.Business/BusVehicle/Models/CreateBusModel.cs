using System.ComponentModel.DataAnnotations;

namespace FBus.Business.BusVehicle.Models
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