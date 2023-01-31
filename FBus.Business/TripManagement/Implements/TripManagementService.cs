using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.BusVehicleManagement.Interfaces;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.RouteManagement.Interfaces;
using FBus.Business.StationManagement.Implements;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.TripManagement.Interfaces;
using FBus.Business.TripManagement.SearchModel;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace FBus.Business.TripManagement.Implements
{
    public class TripManagementService : BaseService, ITripManagementService
    {
        IRouteManagementService _routeManagementService;
        private INotificationService _notificationService;
        private IConfiguration _configuration;

        public IUnitOfWork UnitOfWork { get; }

        public TripManagementService(IUnitOfWork unitOfWork, IRouteManagementService routeManagementService,
                                    INotificationService notificationService, IConfiguration configuration) : base(unitOfWork)
        {
            _routeManagementService = routeManagementService;
            _notificationService = notificationService;
            _configuration = configuration;
            UnitOfWork = unitOfWork;
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
                                x.DriverId.Equals(model.DriverId) &&
                                x.RouteId.Equals(model.RouteId) &&
                                x.BusVehicleId.Equals(model.BusId) &&
                                (
                                    (x.TimeStart.CompareTo(start) <= 0 && start.CompareTo(x.TimeEnd) <= 0) ||
                                    (x.TimeStart.CompareTo(end) <= 0 && end.CompareTo(x.TimeEnd) <= 0) ||
                                    (start.CompareTo(x.TimeStart) < 0 && x.TimeStart.CompareTo(end) <= 0)
                                ))
                                .FirstOrDefaultAsync()) != null;
                if (!already)
                {
                    already = (await _unitOfWork.TripRepository
                                .Query()
                                .Where(x => x.Date.Equals(model.StartDate.AddDays(i)) &&
                                x.BusVehicleId.Equals(model.BusId) &&
                                (
                                    (x.TimeStart.CompareTo(start) <= 0 && start.CompareTo(x.TimeEnd) <= 0) ||
                                    (x.TimeStart.CompareTo(end) <= 0 && end.CompareTo(x.TimeEnd) <= 0) ||
                                    (start.CompareTo(x.TimeStart) < 0 && x.TimeStart.CompareTo(end) <= 0)
                                ))
                                .FirstOrDefaultAsync()) != null;
                }
                else
                {
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = Message.AlreadyExist
                    };
                }
                if (!already)
                {
                    already = (await _unitOfWork.TripRepository
                                .Query()
                                .Where(x => x.Date.Equals(model.StartDate.AddDays(i)) &&
                                x.DriverId.Equals(model.DriverId) &&
                                (
                                    (x.TimeStart.CompareTo(start) <= 0 && start.CompareTo(x.TimeEnd) <= 0) ||
                                    (x.TimeStart.CompareTo(end) <= 0 && end.CompareTo(x.TimeEnd) <= 0) ||
                                    (start.CompareTo(x.TimeStart) < 0 && x.TimeStart.CompareTo(end) <= 0)
                                ))
                                .FirstOrDefaultAsync()) != null;
                }
                else
                {
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = Message.AlreadyExist
                    };
                }
                if (!already)
                {
                    var entity = new Trip()
                    {
                        BusVehicleId = model.BusId,
                        DriverId = model.DriverId,
                        RouteId = model.RouteId,
                        Date = model.StartDate.AddDays(i),
                        TimeEnd = TimeSpan.Parse(model.TimeEnd),
                        TimeStart = TimeSpan.Parse(model.TimeStart),
                        Status = model.DriverId != null ? (int)TripStatus.Waiting : (int)TripStatus.NoDriver,
                        TripId = Guid.NewGuid()
                    };
                    await _unitOfWork.TripRepository.Add(entity);
                }
                else
                {
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = Message.AlreadyExist
                    };
                }
            }

            // update status driver for being assign trip
            if (model.DriverId != null)
            {
                var driver = await _unitOfWork.DriverRepository.GetById(model.DriverId.Value);
                driver.Status = (int)DriverStatus.Assigned;
                _unitOfWork.DriverRepository.Update(driver);
            }

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
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
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

        public async Task<Response> GetStudentTrips(Guid id)
        {
            var entity = await _unitOfWork.TripRepository.GetById(id);
            if (entity != null)
            {
                var routeStationList = await _unitOfWork.RouteStationRepository.Query().Where(x => x.RouteId == entity.RouteId).ToListAsync();
                List<TripGetStudentTripModel> resultList = new List<TripGetStudentTripModel>();
                foreach (var routeStation in routeStationList)
                {
                    TripGetStudentTripModel result = new TripGetStudentTripModel();
                    result.Station = (await _unitOfWork.StationRepository.GetById(routeStation.StationId)).AsViewModel();
                    result.Count = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.StationId == routeStation.StationId).CountAsync();
                    resultList.Add(result);
                }

                return new()
                {
                    StatusCode = (int)StatusCode.Ok,
                    Data = resultList,
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
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
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
                .Where(x => driverID == null || (driverID != null && x.DriverId != null && x.DriverId.Value.Equals(driverID))).ToListAsync();
            var resultList = new List<TripViewModel>();
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
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
                if (entity.Status == (int)TripStatus.NoDriver &&
                    entity.DriverId == null &&
                    model.DriverId != null
                )
                {
                    entity.Status = (int)TripStatus.Waiting;
                }
                else
                {
                    entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
                }

                entity.BusVehicleId = UpdateTypeOfNullAbleObject<Guid>(entity.BusVehicleId, model.BusId);
                entity.RouteId = UpdateTypeOfNullAbleObject<Guid>(entity.RouteId, model.RouteId);
                entity.DriverId = UpdateTypeOfNotNullAbleObject<Guid>(entity.DriverId, model.DriverId);
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
                            .Where(t => t.DriverId != null && t.DriverId.Value == Guid.Parse(id))
                            .Where(t =>
                                (t.Date.Day.CompareTo(DateTime.UtcNow.AddHours(7).Day) < 0 && t.Date.Month.CompareTo(DateTime.UtcNow.AddHours(7).Month) == 0 && t.Date.Year.CompareTo(DateTime.UtcNow.AddHours(7).Year) == 0) ||
                                (t.Date.Month.CompareTo(DateTime.UtcNow.AddHours(7).Month) < 0 && t.Date.Year.CompareTo(DateTime.UtcNow.AddHours(7).Year) == 0) ||
                                (t.Date.Year.CompareTo(DateTime.UtcNow.AddHours(7).Year) < 0) ||
                                (
                                    t.Date.Day.CompareTo(DateTime.UtcNow.AddHours(7).Day) == 0 &&
                                    t.Date.Month.CompareTo(DateTime.UtcNow.AddHours(7).Month) == 0 &&
                                    t.Date.Year.CompareTo(DateTime.UtcNow.AddHours(7).Year) == 0 &&
                                    t.TimeEnd.CompareTo(DateTime.UtcNow.AddHours(7).TimeOfDay) < 0
                                )
                            ).ToListAsync();
                var resultList = new List<TripViewModel>();
                foreach (var entity in th)
                {
                    var result = entity.AsViewModel();
                    result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                    result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                    result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                    var studentTrips = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                    result.Rate = (float?)studentTrips.Average(x => x.Rate);
                    resultList.Add(result);
                }
                return new()
                {
                    StatusCode = (int)StatusCode.Ok,
                    Message = Message.GetListSuccess,
                    Data = resultList,
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
                        .Where(t => t.DriverId != null && t.DriverId.Value == Guid.Parse(id))
                        .Where(t => t.Date.Day.CompareTo(DateTime.UtcNow.AddHours(7).Day) >= 0)
                        .Where(t => t.Date.Month.CompareTo(DateTime.UtcNow.AddHours(7).Month) >= 0)
                        .Where(t => t.Date.Year.CompareTo(DateTime.UtcNow.AddHours(7).Year) >= 0)
                        .ToListAsync();
            var resultList = new List<TripViewModel>();
            foreach (var entity in th)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                var studentTrips = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                result.Rate = (float?)studentTrips.Average(x => x.Rate);
                if (result.Date > DateTime.UtcNow.AddHours(7).Date || (result.Date == DateTime.UtcNow.AddHours(7).Date && result.TimeStart >= DateTime.UtcNow.AddHours(7).TimeOfDay))
                {
                    resultList.Add(result);
                }
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = resultList,
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
            _unitOfWork.TripRepository.UpdateFieldsChange(tripInfo, x => x.DriverId);

            // Update status driver to Assigned
            var swapDriver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.SwappedDriverId));
            swapDriver.Status = (int)DriverStatus.Assigned;
            _unitOfWork.DriverRepository.UpdateFieldsChange(swapDriver, x => x.Status);

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
                Title = "Nhận chuyến mới",
                Content = "Bạn vừa được cập nhật thông tin chuyến mới. Vui lòng kiểm tra lịch trình di chuyển",
                Type = NotificationType.BeAssignedNewTrip
            };
            await _notificationService.SaveNotification(saveNoti1, Role.Driver);

            var requestDriver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.RequestDriverId));

            if (!string.IsNullOrEmpty(swapDriver.NotifyToken))
            {
                await _notificationService.SendNotification(
                    swapDriver.NotifyToken,
                    "Nhận chuyến mới",
                    "Bạn vừa được cập nhật thông tin chuyến mới. Vui lòng kiểm tra lịch trình di chuyển"
                );
            }

            if (!string.IsNullOrEmpty(requestDriver.NotifyToken))
            {
                await _notificationService.SendNotification(
                    requestDriver.NotifyToken,
                    "Yêu cầu đổi tài xế",
                    "Yêu cầu đổi tài xế cho tuyến của bạn đã thành công"
                );
            }

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

            var isAvailiableTime = model.RequestTime.Date.CompareTo(tripInfo.Date) < 0 ||
                                    (
                                        model.RequestTime.TimeOfDay.CompareTo(tripInfo.TimeStart.Subtract(TimeSpan.FromMinutes(30))) <= 0 &&
                                        model.RequestTime.Date.CompareTo(tripInfo.Date) == 0
                                    );

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
                                    x.DriverId != null && x.DriverId.Value != driver.DriverId &&
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
                                )
                            .Join(_unitOfWork.DriverRepository.Query(),
                                _ => _.DriverId.Value,
                                driver => driver.DriverId,
                                (_, driver) => driver.AsDriverViewModel()
                            )
                            .Distinct()
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
                Type = NotificationType.SendRequest,
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
            if (!string.IsNullOrEmpty(driver.NotifyToken))
            {
                await _notificationService.SendNotification(
                    driver.NotifyToken,
                    "Gửi yêu cầu đổi tài xế cho tuyến",
                    "Bạn vừa gửi thành công yêu cầu đổi tài cho tuyến"
                );
            }

            // send Noti for admin
            var adminList = await _unitOfWork.AdminRepository
                        .Query()
                        .Where(x => x.NotifyToken != null || x.NotifyToken != "")
                        .Select(x => new
                        {
                            AdminId = x.AdminId,
                            NotifyToken = x.NotifyToken
                        })
                        .ToListAsync();

            foreach (var admin in adminList)
            {
                await _notificationService.SendNotification(
                    admin.NotifyToken,
                    "Bạn vừa nhận được yêu cầu đổi tài xế",
                    $"Tài xế {driver.FullName} vừa gửi yêu cầu đổi tuyến cho quản trị viên"
                );
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.CustomContent("Thành công!"),
            };
        }

        public async Task<Response> CheckInTripForDriver(string qrCode, string driverId)
        {
            var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(driverId));
            if (driver == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Tài xế không hợp lệ!")
                };
            }

            var busId = new Guid(DecryptString(qrCode));
            var bus = await _unitOfWork.BusRepository.GetById(busId);
            if (bus == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Xe buýt không hợp lệ!")
                };
            }

            var currentDate = DateTime.UtcNow.AddHours(7);
            var dateCheck = currentDate.Date;
            var timeCheck = currentDate.TimeOfDay;

            var availableTrip = await _unitOfWork.TripRepository
                                .Query()
                                .Where(x =>
                                    x.BusVehicleId == busId &&
                                    x.DriverId != null && x.DriverId.Value == Guid.Parse(driverId) &&
                                    x.Date.CompareTo(dateCheck) == 0 &&
                                    x.TimeStart <= timeCheck &&
                                    x.TimeEnd >= timeCheck &&
                                    x.Status == (int)TripStatus.Waiting)
                                .FirstOrDefaultAsync();

            if (availableTrip == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Không tìm thấy tuyến phù hợp!")
                };
            }

            availableTrip.Status = (int)TripStatus.Active;
            _unitOfWork.TripRepository.Update(availableTrip);

            driver.Status = (int)DriverStatus.Running;
            _unitOfWork.DriverRepository.Update(driver);

            bus.Status = (int)BusStatus.Running;
            _unitOfWork.BusRepository.Update(bus);

            await _unitOfWork.SaveChangesAsync();

            // Send notification to driver
            NoticationModel saveNoti = new NoticationModel
            {
                EntityId = driverId,
                Title = "Checkin",
                Content = "Bạn vừa checkin thành công!",
                Type = NotificationType.Checkin,
            };
            await _notificationService.SaveNotification(saveNoti, Role.Driver);

            // Send notification to client
            if (!string.IsNullOrEmpty(driver.NotifyToken))
            {
                await _notificationService.SendNotification(
                              driver.NotifyToken,
                              "Checkin",
                              "Bạn vừa checkin thành công!"
                          );
            }

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CustomContent("Bạn vừa checkin thành công!")
            };
        }

        public string DecryptString(string cipherText)
        {
            string key = _configuration["QRKey"];
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((System.IO.Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((System.IO.Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public async Task<Response> MakeReadRequest(string id)
        {
            var noti = await _unitOfWork.ShiftRepository.GetById(Guid.Parse(id));
            if (noti == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Yêu cầu không hợp lệ")
                };
            }

            noti.Status = (int)ShiftStatus.Checked;
            _unitOfWork.ShiftRepository.Update(noti);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess
            };
        }

        public async Task<Response> GetFeedback(Guid id)
        {
            var feedBackList = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == id).Select(x => new TripFeedbackModel()
            {
                Connent = x.Feedback,
                Rate = Convert.ToDouble(x.Rate)
            }).ToListAsync();
            if (feedBackList == null || feedBackList.Count == 0)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Data = feedBackList,
                Message = Message.GetListSuccess
            };
        }

        public async Task<Response> GetCurrent(Guid id)
        {
            var entities = await _unitOfWork.TripRepository.Query()
                .Where(x => x.DriverId.Equals(id) && x.Status != (int)TripStatus.Disable).ToListAsync();
            var currentDate = DateTime.UtcNow.AddHours(7);
            var minTimeSpan = TimeSpan.MaxValue.TotalMinutes;
            TripViewModel rs = null;
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x => x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => entity.DriverId != null && x.DriverId == entity.DriverId.Value).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                var studentTrips = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                result.Rate = (float?)studentTrips.Average(x => x.Rate);
                var timeSpan = Math.Abs(result.Date.AddMinutes(result.TimeStart.TotalMinutes).Subtract(currentDate).TotalMinutes);
                if (minTimeSpan > timeSpan)
                {
                    rs = result;
                    minTimeSpan = timeSpan;
                }
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = rs,
                Message = Message.GetListSuccess
            };
        }

    }
}
