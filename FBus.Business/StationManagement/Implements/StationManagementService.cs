using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StationManagement.SearchModel;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StationManagement.Implements
{
    public class StationManagementService : BaseService, IStationManagementService
    {
        public StationManagementService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Response> Create(StationSearchModel model)
        {
            bool already = (await _unitOfWork.StationRepository.Query().Where(x => x.Latitude == model.Latitude && x.Longitude == model.Longitude).FirstOrDefaultAsync())!= null;
            if (already)
            {
                return new()
                {
                    StatusCode =(int) StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };
            }
            var entity = new Station()
            {
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Name = model.Name,
                Status = 1,
                StationId = Guid.NewGuid()
            };
            await _unitOfWork.StationRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CreatedSuccess
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.StationRepository.GetById(id);
            if(entity!= null)
            {
                entity.Status = 0;
                _unitOfWork.StationRepository.Update(entity);
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
            var entity = await _unitOfWork.StationRepository.GetById(id);
            if( entity!= null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.Ok,
                    Data = entity.AsViewModel(),
                    Message= Message.GetDetailsSuccess
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
            var entities = await _unitOfWork.StationRepository.Query().Select(x => x.AsViewModel()).ToListAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = entities,
                Message= Message.GetListSuccess
            };
        }

        public async Task<Response> Update(StationUpdateModel model, Guid id)
        {
            var entity= await _unitOfWork.StationRepository.GetById(id);
            if(entity!= null)
            {
                entity.Longitude= UpdateTypeOfNotNullAbleObject<decimal>(entity.Longitude, model.Longitude);
                entity.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(entity.Latitude, model.Latitude);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
                entity.Name= UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
                entity.Address= UpdateTypeOfNullAbleObject<string>(entity.Address, model.Address);
                _unitOfWork.StationRepository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new()
                {
                    StatusCode= (int)StatusCode.Success,
                    Message= Message.UpdatedSuccess
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
