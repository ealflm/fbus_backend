using System.ComponentModel.DataAnnotations;
using FBus.Business.BaseBusiness.CommonModel;

namespace FBus.Business.StudentManagement.Models
{
    public class UpdateStudentModel : FileModel
    {
        public string FullName { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public string NotifyToken { get; set; }
        public bool AutomaticScheduling { get; set; }
        public int Status { get; set; }
    }
}