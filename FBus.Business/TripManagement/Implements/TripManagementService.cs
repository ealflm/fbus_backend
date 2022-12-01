using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.BusVehicleManagement.Interfaces;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.RouteManagement.Interfaces;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.SearchModel;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.TripManagement.Implements
{
    public class TripManagementService : BaseService, ITripManagementService
    {
        IRouteManagementService _routeManagementService;
        private INotificationService _notificationService;

        public TripManagementService(IUnitOfWork unitOfWork, IRouteManagementService routeManagementService, INotificationService notificationService) : base(unitOfWork)
        {
            _routeManagementService = routeManagementService;
            _notificationService = notificationService;
        }

        public async Task<Response> Create(TripSearchModel model)
        {
            TimeSpan start = TimeSpan.Parse(model.TimeStart);
            TimeSpan end = TimeSpan.Parse(model.TimeEnd);
            for (int i = 0; i <= (model.EndDate - model.StartDate).Days; i++)
            {
                bool already = (await _unitOfWork.TripRepository
                                .Query()
                                .Where(x => x.Date.Equals(model.StartDate.AddDays(i)) &&
                                x.RouteId.Equals(model.RouteId) &&
                                x.BusVehicleId.Equals(model.BusId) &&
                                (
                                    (x.TimeStart.CompareTo(start) <= 0 && start.CompareTo(x.TimeEnd) <= 0) ||
                                    (x.TimeStart.CompareTo(end) <= 0 && end.CompareTo(x.TimeEnd) <= 0) ||
                                    (start.CompareTo(x.TimeStart) < 0 && x.TimeStart.CompareTo(end) <= 0)
                                ))
                                .FirstOrDefaultAsync()) != null;

                if (!already && model.StartDate.AddDays(i).DayOfWeek != DayOfWeek.Sunday)
                {
                    var entity = new Trip()
                    {
                        BusVehicleId = model.BusId,
                        DriverId = model.DriverId,
                        RouteId = model.RouteId,
                        Date = model.StartDate.AddDays(i),
                        TimeEnd = TimeSpan.Parse(model.TimeEnd),
                        TimeStart = TimeSpan.Parse(model.TimeStart),
                        Status = (int)TripStatus.Active,
                        TripId = Guid.NewGuid()
                    };
                    await _unitOfWork.TripRepository.Add(entity);
                }
            }

            // update status driver for being assign trip
            var driver = await _unitOfWork.DriverRepository.GetById(model.DriverId);
            driver.Status = (int)DriverStatus.Assigned;
            _unitOfWork.DriverRepository.Update(driver);

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CreatedSuccess
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.TripRepository.GetById(id);
            if (entity != null)
            {
                entity.Status = (int)TripStatus.Disable;
                _unitOfWork.TripRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new()
                {
                    StatusCode = (int)StatusCode.Success,
                    Message = Message.UpdatedSuccess
                };
            }
            return new()
            {
                StatusCode = (int)StatusCode.NotFound,
                Message = Message.NotFound
            };
        }

        public async Task<Response> Get(Guid id)
        {
            var entity = await _unitOfWork.TripRepository.GetById(id);
            if (entity != null)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => x.DriverId == entity.DriverId).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                return new()
                {
                    StatusCode = (int)StatusCode.Ok,
                    Data = result,
                    Message = Message.GetDetailsSuccess
                };
            }
            return new()
            {
                StatusCode = (int)StatusCode.NotFound,
                Message = Message.NotFound
            };
        }

        public async Task<Response> GetListByRoute(Guid? routeID, DateTime? date)
        {
            var entities = await _unitOfWork.TripRepository.Query()
                .Where(x => date == null || (date != null && x.Date.Equals(date)))
                .Where(x => routeID == null || (routeID != null && x.RouteId.Equals(routeID))).ToListAsync();
            var resultList = new List<TripViewModel>();
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => x.DriverId == entity.DriverId).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                var studentTrips = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                result.Rate = (float?)studentTrips.Average(x => x.Rate);
                resultList.Add(result);
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList,
                Message = Message.GetListSuccess
            };
        }

        public async Task<Response> GetList(Guid? busID, Guid? driverID)
        {
            var entities = await _unitOfWork.TripRepository.Query()
                .Where(x => busID == null || (busID != null && x.BusVehicleId.Equals(busID)))
                .Where(x => driverID == null || (driverID != null && x.DriverId.Equals(driverID))).ToListAsync();
            var resultList = new List<TripViewModel>();
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => x.DriverId == entity.DriverId).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                var studentTrips = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                result.Rate = (float?)studentTrips.Average(x => x.Rate);
                resultList.Add(result);
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList,
                Message = Message.GetListSuccess
            };
        }


        public async Task<Response> Update(TripUpdateModel model, Guid id)
        {
            var entity = await _unitOfWork.TripRepository.GetById(id);
            if (entity != null)
            {
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
                entity.BusVehicleId = UpdateTypeOfNullAbleObject<Guid>(entity.BusVehicleId, model.BusId);
                entity.RouteId = UpdateTypeOfNullAbleObject<Guid>(entity.RouteId, model.RouteId);
                entity.DriverId = UpdateTypeOfNullAbleObject<Guid>(entity.DriverId, model.DriverId);
                entity.Date = UpdateTypeOfNullAbleObject<DateTime>(entity.Date, model.Date);
                entity.TimeEnd = UpdateTypeOfNullAbleObject<TimeSpan>(entity.TimeEnd, TimeSpan.Parse(model.TimeEnd));
                entity.TimeStart = UpdateTypeOfNullAbleObject<TimeSpan>(entity.TimeStart, TimeSpan.Parse(model.TimeStart));
                _unitOfWork.TripRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new()
                {
                    StatusCode = (int)StatusCode.Success,
                    Message = Message.UpdatedSuccess
                };
            }
            return new()
            {
                StatusCode = (int)StatusCode.NotFound,
                Message = Message.NotFound
            };
        }

        public async Task<Response> GetHistoricalTrip(string id, Role role)
        {
            if (role == Role.Driver)
            {
                var th = await _unitOfWork.TripRepository
                            .Query()
                            .Where(t => t.DriverId == Guid.Parse(id))
                            .Where(t =>
                                (t.Date.Day.CompareTo(DateTime.UtcNow.Day) < 0 && t.Date.Month.CompareTo(DateTime.UtcNow.Month) == 0 && t.Date.Year.CompareTo(DateTime.UtcNow.Year) == 0) ||
                                (t.Date.Month.CompareTo(DateTime.UtcNow.Month) < 0 && t.Date.Year.CompareTo(DateTime.UtcNow.Year) == 0) ||
                                (t.Date.Year.CompareTo(DateTime.UtcNow.Year) < 0) ||
                                (
                                    t.Date.Day.CompareTo(DateTime.UtcNow.Day) == 0 &&
                                    t.Date.Month.CompareTo(DateTime.UtcNow.Month) == 0 &&
                                    t.Date.Year.CompareTo(DateTime.UtcNow.Year) == 0 &&
                                    t.TimeEnd.CompareTo(DateTime.UtcNow.TimeOfDay) < 0
                                )
                            )
                            .Select(t => t.AsViewModel())
                            .ToListAsync();

                return new()
                {
                    StatusCode = (int)StatusCode.Ok,
                    Message = Message.GetListSuccess,
                    Data = th,
                };
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess
            };
        }

        public async Task<Response> GetDriverTripSchedules(string id)
        {
            var th = await _unitOfWork.TripRepository
                        .Query()
                        .Where(t => t.DriverId == Guid.Parse(id))
                        .Where(t => t.Date.Day.CompareTo(DateTime.UtcNow.Day) >= 0)
                        .Where(t => t.Date.Month.CompareTo(DateTime.UtcNow.Month) >= 0)
                        .Where(t => t.Date.Year.CompareTo(DateTime.UtcNow.Year) >= 0)
                        .Where(t => t.TimeStart.CompareTo(DateTime.UtcNow.TimeOfDay) >= 0)
                        .Select(t => t.AsViewModel())
                        .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = th,
            };
        }

        public async Task<Response> DoSwapDriver(SwapDriverModel model)
        {
            var tripInfo = await _unitOfWork.TripRepository.GetById(Guid.Parse(model.TripId));
            if (tripInfo == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Thông tin chuyến đi không hợp lệ!")
                };
            }

            // Update new driver for trip
            tripInfo.DriverId = Guid.Parse(model.SwappedDriverId);
            _unitOfWork.TripRepository.Update(tripInfo);

            // Update status driver to Assigned
            var swapDriver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.SwappedDriverId));
            swapDriver.Status = (int)DriverStatus.Assigned;
            _unitOfWork.DriverRepository.Update(swapDriver);

            await _unitOfWork.SaveChangesAsync();

            // Send notification to client
            NoticationModel saveNoti = new NoticationModel
            {
                EntityId = model.RequestDriverId,
                Title = "Yêu cầu đổi tài xế cho tuyến xe buýt",
                Content = "Yêu cầu đổi tài xế cho tuyến của bạn đã thành công",
                Type = NotificationType.SwapDriver
            };
            await _notificationService.SaveNotification(saveNoti, Role.Driver);

            // Send notification to client
            NoticationModel saveNoti1 = new NoticationModel
            {
                EntityId = swapDriver.DriverId.ToString(),
                Title = "Yêu cầu đổi tài xế cho tuyến xe buýt",
                Content = "Bạn vừa được cập nhật thông tin chuyến mới. Vui lòng kiểm tra lịch trình di chuyển",
                Type = NotificationType.SwapDriver
            };
            await _notificationService.SaveNotification(saveNoti1, Role.Driver);

            var requestDriver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.RequestDriverId));

            await _notificationService.SendNotification(
                swapDriver.NotifyToken,
                "Yêu cầu đổi tài xế",
                "Bạn vừa được cập nhật thông tin chuyến mới. Vui lòng kiểm tra lịch trình di chuyển"
            );

            await _notificationService.SendNotification(
                requestDriver.NotifyToken,
                "Yêu cầu đổi tài xế",
                "Yêu cầu đổi tài xế cho tuyến của bạn đã thành công"
            );

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CustomContent("Đổi tài xế thành công!")
            };
        }

        public async Task<Response> CheckAvailableRequestTime(RequestTimeModel model)
        {
            var tripInfo = await _unitOfWork.TripRepository.GetById(Guid.Parse(model.TripId));
            if (tripInfo == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Thông tin chuyến đi không hợp lệ!")
                };
            }

            var isAvailiableTime = model.RequestTime.CompareTo(tripInfo.TimeStart.Subtract(TimeSpan.FromMinutes(30))) <= 0;

            if (!isAvailiableTime)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Đã quá thời hạn yêu cầu đổi tài xế cho chuyến của bạn!")
                };
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thời gian hợp lệ!")
            };
        }

        public async Task<Response> GetAvailabelSwappingDriverList(AvailableSwappingDriverModel model)
        {
            var trip = await _unitOfWork.TripRepository.GetById(Guid.Parse(model.TripId));
            var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.DriverId));

            var driverLst = await _unitOfWork.DriverRepository
                            .Query()
                            .Where(x => x.DriverId != driver.DriverId)
                            .Where(x => x.Status == (int)DriverStatus.Active)
                            .Select(x => x.AsDriverViewModel())
                            .ToListAsync();

            dynamic lst = null;
            if (trip != null && driver != null)
            {
                lst = await _unitOfWork.TripRepository
                            .Query()
                            .Where(x =>
                                    x.DriverId != driver.DriverId &&
                                    (
                                        !(
                                            trip.TimeStart.CompareTo(x.TimeStart) <= 0 &&
                                            x.TimeStart.CompareTo(trip.TimeEnd) <= 0
                                        )
                                        ||
                                        !(
                                            trip.TimeStart.CompareTo(x.TimeEnd) <= 0 &&
                                            x.TimeEnd.CompareTo(trip.TimeEnd) <= 0
                                        )

                                    )
                                // &&
                                // (
                                //     x.TimeEnd.Subtract(TimeSpan.FromMinutes(10)).CompareTo(trip.TimeStart) <= 0 ||
                                //     trip.TimeEnd.CompareTo(x.TimeStart.Subtract(TimeSpan.FromMinutes(10))) <= 0
                                // )
                                )
                            .Join(_unitOfWork.DriverRepository.Query(),
                                _ => _.DriverId,
                                driver => driver.DriverId,
                                (_, driver) => driver.AsDriverViewModel()
                            )
                            .ToListAsync();


            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công!"),
                Data = new
                {
                    HasNoTrip = driverLst,
                    HasTrip = lst
                }
            };
        }

        public async Task<Response> SendRequestToSwapDriver(RequestSwapDriverModel model)
        {
            RequestTimeModel checktimeModel = new RequestTimeModel
            {
                TripId = model.TripId,
                RequestTime = model.RequestTime
            };
            var checkRequestTime = await CheckAvailableRequestTime(checktimeModel);
            if (checkRequestTime.StatusCode != (int)StatusCode.Ok)
            {
                return checkRequestTime;
            }

            Shift shift = new Shift
            {
                ShiftId = Guid.NewGuid(),
                DriverId = Guid.Parse(model.DriverId),
                TripId = Guid.Parse(model.TripId),
                RequestTime = model.RequestTime,
                Content = model.Content,
                Type = "SendRequest",
                Status = (int)ShiftStatus.UnChecked
            };

            await _unitOfWork.ShiftRepository.Add(shift);
            await _unitOfWork.SaveChangesAsync();

            NoticationModel saveNoti = new NoticationModel
            {
                EntityId = model.DriverId,
                Title = "Gửi yêu cầu đổi tài xế cho tuyến",
                Content = "Bạn vừa gửi thành công yêu cầu đổi tài cho tuyến",
                Type = NotificationType.SendRequest
            };
            await _notificationService.SaveNotification(saveNoti, Role.Driver);

            var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.DriverId));
            await _notificationService.SendNotification(
                driver.NotifyToken,
                "Gửi yêu cầu đổi tài xế cho tuyến",
                "Bạn vừa gửi thành công yêu cầu đổi tài cho tuyến"
            );

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công!"),
            };
        }
    }
}
