using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentManagement.Models;
using FBus.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.StudentManagement.Implements
{
    public class StudentService : BaseService, IStudentService
    {
        private IAzureBlobService _azureBlobService;

        public StudentService(IUnitOfWork unitOfWork, IAzureBlobService azureBlobService) : base(unitOfWork)
        {
            _azureBlobService = azureBlobService;
        }

        public async Task<Response> Disable(string id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (student == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            student.Status = (int)StudentStatus.Disable;
            _unitOfWork.StudentRepository.Update(student);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess,
            };
        }

        public async Task<Response> GetDetails(string id)
        {
            var student = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (student == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            // Cách join bảng với nhau
            var studentTrip = await _unitOfWork.StudentTripRepository
                                .Query()
                                .Where(s => s.StudentId == Guid.Parse(id))
                                .Join(_unitOfWork.TripRepository.Query(),
                                    studentTrip => studentTrip.TripId,
                                    trip => trip.TripId,
                                    (studentTrip, trip) => new { StudentTrip = studentTrip, Trip = trip }
                                )
                                .Join(_unitOfWork.StationRepository.Query(),
                                    _ => _.StudentTrip.StationId,
                                    station => station.StationId,
                                    (_, station) => new { StudentTrip = _.StudentTrip, Trip = _.Trip, Station = station }
                                )
                                .Join(_unitOfWork.RouteRepository.Query(),
                                    _ => _.Trip.RouteId,
                                    route => route.RouteId,
                                    (_, route) => new { StudentTrip = _.StudentTrip, Trip = _.Trip, Station = _.Station, Route = route }
                                )
                                .Join(_unitOfWork.DriverRepository.Query(),
                                    _ => _.Trip.DriverId,
                                    driver => driver.DriverId,
                                    (_, driver) => new { StudentTrip = _.StudentTrip, Trip = _.Trip, Station = _.Station, Route = _.Route, Driver = driver }
                                )
                                .Join(_unitOfWork.BusRepository.Query(),
                                    _ => _.Trip.BusVehicleId,
                                    busVehicle => busVehicle.BusVehicleId,
                                    (_, busVehicle) => new { StudentTrip = _.StudentTrip, Trip = _.Trip, Station = _.Station, Route = _.Route, Driver = _.Driver, BusVehicle = busVehicle }
                                )
                                .Select(_ => new
                                {
                                    StudentTripInfo = _.StudentTrip,
                                    StartedStationInfo = _.Station,
                                    TripInfo = _.Trip,
                                    RouteInfo = _.Route,
                                    BusVehicleInfo = _.BusVehicle,
                                    DriverInfo = _.Driver,
                                })
                                .ToListAsync();

            // Khởi tạo danh sách stations trong route
            var data = new List<object>();


            foreach (var item in studentTrip)
            {
                var routeStations = await _unitOfWork.RouteStationRepository
                                    .Query()
                                    .Where(_ => _.RouteId == item.RouteInfo.RouteId)
                                    .Join(_unitOfWork.StationRepository.Query(),
                                        routeStation => routeStation.StationId,
                                        station => station.StationId,
                                        (routeStation, station) => new
                                        {
                                            OrderNumber = routeStation.OrderNumber,
                                            Distance = routeStation.Distance,
                                            Id = station.StationId,
                                            Name = station.Name,
                                            Address = station.Address,
                                            Longitude = station.Longitude,
                                            Latitude = station.Latitude,
                                            Status = station.Status
                                        }
                                    )
                                    .ToListAsync();
                data.Add(
                    new
                    {
                        StudentTripInfo = new
                        {
                            StudentTripId = item.StudentTripInfo.StudentTripId,
                            CopyOfRoute = item.StudentTripInfo.CopyOfRoute,
                            Rate = item.StudentTripInfo.Rate,
                            Feedback = item.StudentTripInfo.Feedback,
                            CreateDate = item.StudentTripInfo.CreateDate,
                            ModifyDate = item.StudentTripInfo.ModifyDate,
                            Type = item.StudentTripInfo.Type,
                            Status = item.StudentTripInfo.Status
                        },
                        StartedStationInfo = item.StartedStationInfo.AsViewModel(),
                        TripInfo = item.TripInfo.AsViewModel(),
                        RouteInfo = item.RouteInfo.AsViewModel(),
                        BusVehicleInfo = item.BusVehicleInfo.AsBusViewModel(),
                        DriverInfo = item.DriverInfo.AsDriverViewModel(),
                        RouteStationInfo = routeStations
                    }
                );

            }


            // Cách custom object
            // var studentTripList = await _unitOfWork.StudentTripRepository
            //                     .Query()
            //                     .Where(_ => _.StudentId == Guid.Parse(id))
            //                     .Join(_unitOfWork.StationRepository.Query(),
            //                         studentTrip => studentTrip.StationId,
            //                         station => station.StationId,
            //                         (studentTrip, station) => new { StudentTrip = studentTrip, Station = station }
            //                     )
            //                     .Select(_ => new
            //                     {
            //                         StudentTripInfo = new
            //                         {
            //                             StudentTripId = _.StudentTrip.StudentTripId,
            //                             CopyOfRoute = _.StudentTrip.CopyOfRoute,
            //                             Rate = _.StudentTrip.Rate,
            //                             Feedback = _.StudentTrip.Feedback,
            //                             CreateDate = _.StudentTrip.CreateDate,
            //                             ModifyDate = _.StudentTrip.ModifyDate,
            //                             Type = _.StudentTrip.Type,
            //                             Status = _.StudentTrip.Status
            //                         },
            //                         StartedStationInfo = new
            //                         {
            //                             Id = _.Station.StationId,
            //                             Name = _.Station.Name,
            //                             Address = _.Station.Address,
            //                             Longitude = _.Station.Longitude,
            //                             Latitude = _.Station.Latitude,
            //                             Status = _.Station.Status
            //                         },
            //                         TripId = _.StudentTrip.TripId
            //                     })
            //                     .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetDetailsSuccess,
                Data = new
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Phone = student.Phone,
                    Email = student.Email,
                    Address = student.Address,
                    PhotoUrl = student.PhotoUrl,
                    AutomaticScheduling = student.AutomaticScheduling,
                    Status = student.Status,
                    data = data
                }
            };
        }

        public async Task<Response> GetList()
        {
            var list = await _unitOfWork.StudentRepository
                            .Query()
                            .Select(s => s.AsStudentViewModel())
                            .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = list
            };
        }

        public async Task<Response> Update(string id, UpdateStudentModel student)
        {
            var stud = await _unitOfWork.StudentRepository.GetById(Guid.Parse(id));
            if (stud == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            stud.FullName = UpdateTypeOfNullAbleObject<string>(stud.FullName, student.FullName);
            stud.Phone = UpdateTypeOfNullAbleObject<string>(stud.Phone, student.Phone);
            stud.Address = UpdateTypeOfNullAbleObject<string>(stud.Address, student.Address);
            stud.PhotoUrl = await _azureBlobService.DeleteFile(student.DeleteFile, AzureBlobContainer.Student, student.PhotoUrl);
            stud.PhotoUrl += await _azureBlobService.UploadFile(student.UploadFile, AzureBlobContainer.Student);
            stud.NotifyToken = UpdateTypeOfNullAbleObject<string>(stud.NotifyToken, student.NotifyToken);
            stud.AutomaticScheduling = UpdateTypeOfNotNullAbleObject<bool>(stud.AutomaticScheduling, student.AutomaticScheduling);
            stud.Status = UpdateTypeOfNotNullAbleObject<int>(stud.Status, student.Status);

            _unitOfWork.StudentRepository.Update(stud);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess,
            };
        }
    }
}