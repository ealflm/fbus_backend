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
        IBusService _busService;
        IRouteManagementService _routeManagementService;
        IDriverService _driveService;
        public TripManagementService(IUnitOfWork unitOfWork, IDriverService driverService, IRouteManagementService routeManagementService, IBusService busService) : base(unitOfWork)
        {
            _busService = busService;
            _routeManagementService = routeManagementService;
            _driveService = driverService;
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
                Status = 1,
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
                entity.Status = 0;
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
                result.Bus = (BusViewModel)(await _busService.GetDetails(entity.BusVehicleId.ToString())).Data;
                result.Driver = (DriverViewModel)(await _driveService.GetDetails(entity.DriverId.ToString())).Data;
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

        public async Task<Response> GetList(DateTime? date)
        {
            var entities = await _unitOfWork.TripRepository.Query()
                .Where(x => date == null || (date != null && x.Date.Equals(date))).ToListAsync();
            var resultList = new List<TripViewModel>();
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Route = (RouteViewModel)(await _routeManagementService.Get(entity.RouteId)).Data;
                result.Bus = (BusViewModel)(await _busService.GetDetails(entity.BusVehicleId.ToString())).Data;
                result.Driver = (DriverViewModel)(await _driveService.GetDetails(entity.DriverId.ToString())).Data;
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
                entity.TimeEnd = UpdateTypeOfNullAbleObject<TimeSpan>(entity.TimeEnd, model.TimeEnd);
                entity.TimeStart = UpdateTypeOfNullAbleObject<TimeSpan>(entity.TimeStart, model.TimeStart);
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
