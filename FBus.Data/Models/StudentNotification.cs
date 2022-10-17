using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class StudentNotification
    {
        public Guid NotificationId { get; set; }
        public Guid StudentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Student Student { get; set; }
    }
}
