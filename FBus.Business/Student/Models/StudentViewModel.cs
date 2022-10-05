using System;

namespace FBus.Business.StudentManagement.Models
{
    public class StudentViewModel
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public bool AutomaticScheduling { get; set; }
        public int Status { get; set; }
    }
}