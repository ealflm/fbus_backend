using System;

namespace FBus.Business.TripManagement.SearchModel
{
    public class RequestSwapDriverModel
    {
        public string TripId { get; set; }
        public string DriverId { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.UtcNow.AddHours(7);
        public string Content { get; set; } = "Lý do sức khỏe cá nhân";
    }

    public class SwapDriverModel
    {
        public string RequestDriverId { get; set; }
        public string SwappedDriverId { get; set; }
        public string TripId { get; set; }
    }

    public class RequestTimeModel
    {
        public string TripId { get; set; }
        public DateTime RequestTime { get; set; }
    }

    public class AvailableSwappingDriverModel
    {
        public string DriverId { get; set; }
        public string TripId { get; set; }
    }
}