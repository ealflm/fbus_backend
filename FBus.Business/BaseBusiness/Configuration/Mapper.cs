using FBus.Business.StudentManagement.Models;
using FBus.Data.Models;

namespace FBus.Business.BaseBusiness.Configuration
{
    public static class EntitiesMapper
    {
        public static StudentViewModel AsStudentViewModel(this Student student)
        {
            return new()
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Phone = student.Phone,
                Email = student.Email,
                Address = student.Address,
                PhotoUrl = student.PhotoUrl,
                AutomaticScheduling = student.AutomaticScheduling,
                Status = student.Status
            };
        }
    }
}