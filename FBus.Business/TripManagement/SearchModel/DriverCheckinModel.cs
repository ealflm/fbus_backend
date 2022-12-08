using System.ComponentModel.DataAnnotations;

namespace FBus.Business.TripManagement.SearchModel
{
    public class DriverCheckinModel
    {
        [Required]
        public string QRCode { get; set; }
        [Required]
        public string DriverId { get; set; }
    }
}