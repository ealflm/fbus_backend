using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.BaseBusiness.ViewModel
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
        public bool IsBan { get; set; }
        public DateTime? DateBan { get; set; }
        public int CountBan { get; set; }
    }
}
