using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Student
    {
        public Student()
        {
            FavoriteRoutes = new HashSet<FavoriteRoute>();
            StartLocations = new HashSet<StartLocation>();
            StudentNotifications = new HashSet<StudentNotification>();
            StudentTrips = new HashSet<StudentTrip>();
        }

        public Guid StudentId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public string NotifyToken { get; set; }
        public string Uid { get; set; }
        public bool AutomaticScheduling { get; set; }
        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CountBan { get; set; }
        public DateTime? DateBan { get; set; }

        public virtual ICollection<FavoriteRoute> FavoriteRoutes { get; set; }
        public virtual ICollection<StartLocation> StartLocations { get; set; }
        public virtual ICollection<StudentNotification> StudentNotifications { get; set; }
        public virtual ICollection<StudentTrip> StudentTrips { get; set; }
    }
}
