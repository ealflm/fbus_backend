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
    }
}