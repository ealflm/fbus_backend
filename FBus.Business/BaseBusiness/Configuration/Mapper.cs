using FBus.Business.BaseBusiness.ViewModel;
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

        public static StationViewModel AsViewModel(this Station item)
        {
            return new()
            {
                Address = item.Address,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Name = item.Name,
                StationId = item.StationId,
                Status = item.Status
            };
        }

        public static RouteViewModel AsViewModel(this Route item)
        {
            return new()
            {
                Distance = item.Distance,
                TotalStation = item.TotalStation,
                RouteId = item.RouteId,
                Name = item.Name,
                Status = item.Status
            };
        }

        public static TripViewModel AsViewModel(this Trip item)
        {
            return new()
            {
                TimeEnd = item.TimeEnd,
                TimeStart = item.TimeStart,
                TripId = item.TripId,
                Date =  item.Date,
                Status = item.Status
            };
        }

        public static StudentTripViewModel AsViewModel(this StudentTrip item)
        {
            return new()
            {
                 CopyOfRoute = item.CopyOfRoute,
                 CreateDate = item.CreateDate, 
                 Feedback = item.Feedback,
                 ModifyDate = item.ModifyDate,
                 Rate = item.Rate,
                 StudentTripId = item.StudentTripId,
                 Type  = item.Type,
                 Status = item.Status
            };
        }

        public static BusViewModel AsBusViewModel(this FBus.Data.Models.BusVehicle item)
        {
            return new()
            {
                BusId = item.BusVehicleId,
                LicensePlates = item.LicensePlates,
                Seat = item.Seat,
                Color = item.Color,
                Status = item.Status
            };
        }

        public static DriverViewModel AsDriverViewModel(this Driver item)
        {
            return new()
            {
                DriverId = item.DriverId,
                FullName = item.FullName,
                Phone = item.Phone,
                PhotoUrl = item.PhotoUrl,
                Address = item.Address,
                Status = item.Status
            };
        }
    }
}