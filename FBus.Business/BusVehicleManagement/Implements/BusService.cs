using System;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BusVehicleManagement.Interfaces;
using FBus.Business.BusVehicleManagement.Models;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.BusVehicleManagement.Implements
{
    public class BusService : BaseService, IBusService
    {
        public BusService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Response> Create(CreateBusModel model)
        {
            var isExisted = await _unitOfWork.BusRepository.Query().AnyAsync(b => b.LicensePlates.Trim() == model.LicensePlates.Trim());
            if (isExisted)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };

            };


            var bus = new FBus.Data.Models.BusVehicle()
            {
                BusVehicleId = Guid.NewGuid(),
                LicensePlates = model.LicensePlates,
                Seat = model.Seat,
                Color = model.Color,
                Status = (int)BusStatus.Active
            };

            await _unitOfWork.BusRepository.Add(bus);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CreatedSuccess
            };
        }

        public Task<Response> Disable(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Response> GetDetails(string id)
        {
            var bus = await _unitOfWork.BusRepository.GetById(Guid.Parse(id));
            if (bus == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetDetailsSuccess,
                Data = bus.AsBusViewModel()
            };
        }

        public async Task<Response> GetList()
        {
            var list = await _unitOfWork.BusRepository
                        .Query()
                        .Select(x => x.AsBusViewModel())
                        .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = list
            };
        }

        public async Task<Response> Update(string id, UpdateBusModel model)
        {
            var bus = await _unitOfWork.BusRepository.GetById(Guid.Parse(id));
            if (bus == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }

            if (!string.IsNullOrEmpty(model.LicensePlates) && bus.LicensePlates.Trim() != model.LicensePlates.Trim())
            {
                var invalidLP = await _unitOfWork.BusRepository
                            .Query()
                            .AnyAsync(b => b.LicensePlates.Trim() == model.LicensePlates.Trim() && b.LicensePlates != bus.LicensePlates);

                if (invalidLP)
                {
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = Message.CustomContent("Biển số xe đã tồn tại.")
                    };
                }
            }

            bus.LicensePlates = UpdateTypeOfNullAbleObject<string>(bus.LicensePlates, model.LicensePlates);
            bus.Seat = UpdateTypeOfNotNullAbleObject<int>(bus.Seat, model.Seat);
            bus.Color = UpdateTypeOfNullAbleObject<string>(bus.Color, model.Color);
            bus.Status = UpdateTypeOfNotNullAbleObject<int>(bus.Status, model.Status);

            _unitOfWork.BusRepository.Update(bus);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess
            };
        }
    }
}