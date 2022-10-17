using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class DriverNotification
    {
        public Guid NotificationId { get; set; }
        public Guid DriverId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Driver Driver { get; set; }
    }
}
