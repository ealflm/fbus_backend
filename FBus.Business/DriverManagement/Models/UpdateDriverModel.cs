using FBus.Business.BaseBusiness.CommonModel;

namespace FBus.Business.DriverManagement.Models
{
    public class UpdateDriverModel : FileModel
    {
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public string Address { get; set; }
        public int? Status { get; set; }
    }
}