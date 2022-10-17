using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentTripManagement.Interfaces;
using FBus.Business.StudentTripManagement.SearchModel;
using FBus.Business.TripManagement.Interfaces;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.Implements
{
    public class StudentTripManagementService : BaseService, IStudentTripManagementService
    {
        private ITripManagementService _tripManagementService;
        private IStationManagementService _stationManagementService;
        private IStudentService _studentManagementService;
        public StudentTripManagementService(IUnitOfWork unitOfWork, ITripManagementService tripManagementService, IStationManagementService stationManagementService, IStudentService studentManagementService) : base(unitOfWork)
        {
            _tripManagementService = tripManagementService;
            _stationManagementService = stationManagementService;
            _studentManagementService = studentManagementService;
        }

        public async Task<Response> Create(StudentTripSearchModel model)
        {
            bool already = (await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId.Equals(model.TripId)).FirstOrDefaultAsync()) != null;
            if (already)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };
            }
            var entity = new StudentTrip()
            {
                CreateDate = DateTime.UtcNow,
                ModifyDate = DateTime.UtcNow,
                StationId = model.StationId,
                StudentId = model.StudentId,
                TripId = model.TripId,
                Type = model.Type,
                Status = (int)StudentTripStatus.Active,
                StudentTripId = Guid.NewGuid()
            };
            var trip = (TripViewModel)(await _tripManagementService.Get(model.TripId)).Data;
            var route = JsonConvert.SerializeObject(trip.Route);
            entity.CopyOfRoute = route;
            await _unitOfWork.StudentTripRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CreatedSuccess
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.StudentTripRepository.GetById(id);
            if (entity != null)
            {
                _unitOfWork.StudentTripRepository.Remove(entity);
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

        public async Task<Response> FeedBack(FeedBackSearchModel model, Guid id)
        {
            var entity = await _unitOfWork.StudentTripRepository.GetById(id);
            if (entity != null)
            {
                entity.Feedback = model.Feedback;
                entity.Rate = model.Rate;
                _unitOfWork.StudentTripRepository.Update(entity);
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
            var entity = await _unitOfWork.StudentTripRepository.GetById(id);
            if (entity != null)
            {
                var result = entity.AsViewModel();
                result.Station = (StationViewModel)(await _stationManagementService.Get(entity.StationId)).Data;
                result.Trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                result.Student = (StudentViewModel)(await _studentManagementService.GetDetails(entity.StudentId.ToString())).Data;
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

        public async Task<Response> GetList()
        {
            var entities = await _unitOfWork.StudentTripRepository.Query().ToListAsync();
            var resultList = new List<StudentTripViewModel>();
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Station = (StationViewModel)(await _stationManagementService.Get(entity.StationId)).Data;
                result.Trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                result.Student = (StudentViewModel)(await _studentManagementService.GetDetails(entity.StudentId.ToString())).Data;
                resultList.Add(result);
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList,
                Message = Message.GetListSuccess
            };
        }

        public async Task<Response> Update(StudentTripUpdateModel model, Guid id)
        {
            var entity = await _unitOfWork.StudentTripRepository.GetById(id);
            if (entity != null)
            {
                entity.ModifyDate = DateTime.UtcNow;
                entity.StationId = UpdateTypeOfNullAbleObject<Guid>(entity.StationId, model.StationId);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
                entity.TripId = UpdateTypeOfNullAbleObject<Guid>(entity.TripId, model.TripId);
                var trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                var route = JsonConvert.SerializeObject(trip.Route);
                entity.CopyOfRoute = route;
                entity.Type = UpdateTypeOfNotNullAbleObject<bool>(entity.Type, model.Type);
                _unitOfWork.StudentTripRepository.Update(entity);
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
    }
}
