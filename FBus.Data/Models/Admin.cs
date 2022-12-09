using System;
using System.Collections.Generic;

namespace FBus.Data.Models
{
    public partial class Admin
    {
        public Guid AdminId { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string NotifyToken { get; set; }
    }
}
