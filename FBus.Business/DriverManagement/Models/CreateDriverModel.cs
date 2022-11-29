using System;
using System.ComponentModel.DataAnnotations;
using FBus.Business.BaseBusiness.CommonModel;

namespace FBus.Business.DriverManagement.Models
{
    public class CreateDriverModel : FileModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
    }
}