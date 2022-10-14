using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.RouteManagement.Interfaces;
using FBus.Business.RouteManagement.SearchModel;
using FBus.Business.StationManagement.Interfaces;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.RouteManagement.Implements
{
    public class RouteManagementService : BaseService, IRouteManagementService
    {
        private IStationManagementService _stationService;
        public RouteManagementService(IUnitOfWork unitOfWork, IStationManagementService stationService) : base(unitOfWork)
        {
            _stationService = stationService;
        }

        public async Task<Response> Create(RouteSearchModel model)
        {
            bool already = (await _unitOfWork.RouteRepository.Query().Where(x => x.Name.Equals(model.Name)).FirstOrDefaultAsync())!= null;
            if (already)
            {
                return new()
                {
                    StatusCode =(int) StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };
            }
            var entity = new Route()
            {
                Distance = model.Distance,
                TotalStation= model.TotalStation,
                Name = model.Name,
                Status = 1,
                RouteId = Guid.NewGuid()
            };
            await _unitOfWork.RouteRepository.Add(entity);
            if (model.StationList.Count > 0)
            {
                for(int i=0;i< model.StationList.Count;i++)
                {
                    var routeStation = new RouteStation()
                    {
                        StationId = model.StationList[i],
                        RouteId = entity.RouteId,
                        OrderNumber = i,
                        Distance = 0
                    };
                    await _unitOfWork.RouteStationRepository.Add(routeStation);
                }
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
            var entity = await _unitOfWork.RouteRepository.GetById(id);
            if(entity!= null)
            {
                entity.Status = 0;
                _unitOfWork.RouteRepository.Update(entity);
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
            var entity = await _unitOfWork.RouteRepository.GetById(id);
            
            if( entity!= null)
            {
                var routeStationList = await _unitOfWork.RouteStationRepository.Query().Where(x => x.RouteId.Equals(entity.RouteId)).ToListAsync();
                var result= entity.AsViewModel();
                result.StationList = new List<StationViewModel>();
                foreach(var x in routeStationList)
                {
                    result.StationList.Add((StationViewModel)_stationService.Get(x.StationId).Result.Data);
                }
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
            var entities = await _unitOfWork.RouteRepository.Query().Select(x => x.AsViewModel()).ToListAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = entities,
                Message= Message.GetListSuccess
            };
        }

      /*  public async Task<Response> Update(RouteUpdateModel model, Guid id)
        {
            var entity= await _unitOfWork.RouteRepository.GetById(id);
            if(entity!= null)
            {
                entity.Longitude= UpdateTypeOfNotNullAbleObject<decimal>(entity.Longitude, model.Longitude);
                entity.Latitude = UpdateTypeOfNotNullAbleObject<decimal>(entity.Latitude, model.Latitude);
                entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
                entity.Name= UpdateTypeOfNullAbleObject<string>(entity.Name, model.Name);
                entity.Address= UpdateTypeOfNullAbleObject<string>(entity.Address, model.Address);
                _unitOfWork.RouteRepository.Update(entity);
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
        }*/
    }
}
