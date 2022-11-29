using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
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
        public TripManagementService(IUnitOfWork unitOfWork, IRouteManagementService routeManagementService) : base(unitOfWork)
        {
            _routeManagementService = routeManagementService;
        }

        public async Task<Response> Create(TripSearchModel model)
        {
            TimeSpan start = TimeSpan.Parse(model.TimeStart);
            TimeSpan end = TimeSpan.Parse(model.TimeEnd);

            bool already = (await _unitOfWork.TripRepository
                            .Query()
                            .Where(x => x.Date.Equals(model.Date) &&
                            x.RouteId.Equals(model.RouteId) &&
                            x.BusVehicleId.Equals(model.BusId) &&
                            (
                                (x.TimeStart.CompareTo(start) <= 0 && start.CompareTo(x.TimeEnd) <= 0) ||
                                (x.TimeStart.CompareTo(end) <= 0 && end.CompareTo(x.TimeEnd) <= 0) ||
                                (start.CompareTo(x.TimeStart) < 0 && x.TimeStart.CompareTo(end) <= 0)
                            ))
                            .FirstOrDefaultAsync()) != null;

            if (already)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };
            }
            var entity = new Trip()
            {
                BusVehicleId = model.BusId,
                DriverId = model.DriverId,
                RouteId = model.RouteId,
                Date = model.Date,
                TimeEnd = TimeSpan.Parse(model.TimeEnd),
                TimeStart = TimeSpan.Parse(model.TimeStart),
                Status = (int)TripStatus.Active,
                TripId = Guid.NewGuid()
            };
            await _unitOfWork.TripRepository.Add(entity);
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
                result.Bus = await _unitOfWork.BusRepository.Query().Where(x => x.BusVehicleId == entity.BusVehicleId).Select(x=> x.AsBusViewModel()).FirstOrDefaultAsync();
                result.Driver = await _unitOfWork.DriverRepository.Query().Where(x => x.DriverId == entity.DriverId).Select(x => x.AsDriverViewModel()).FirstOrDefaultAsync();
                var studentTrips= await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == entity.TripId && x.Rate != null).ToListAsync();
                result.Rate = (float?) studentTrips.Average(x => x.Rate);
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
                .Where(x =>busID  == null || (busID != null && x.BusVehicleId.Equals(busID)))
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
    }
}
