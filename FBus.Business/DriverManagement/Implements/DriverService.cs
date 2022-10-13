using System;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FBus.Business.DriverManagement.Implements
{
    public class DriverService : BaseService, IDriverService
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureBlobService _azureBlobService;
        private readonly ISMSService _smsService;
        public DriverService(IUnitOfWork unitOfWork, IConfiguration configuration, IAzureBlobService azureBlobService, ISMSService smsService) : base(unitOfWork)
        {
            _configuration = configuration;
            _azureBlobService = azureBlobService;
            _smsService = smsService;
        }

        public async Task<Response> Create(CreateDriverModel model)
        {
            var isExisted = await _unitOfWork.DriverRepository
                                    .Query()
                                    .AnyAsync(x => x.Phone == model.Phone);
            if (isExisted)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.AlreadyExist
                };
            }

            string password = GeneratePinCodeAuto(int.Parse(_configuration["GprcPin:Length"]));
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var driver = new Driver()
            {
                DriverId = Guid.NewGuid(),
                FullName = model.FullName,
                Phone = model.Phone,
                PhotoUrl = await _azureBlobService.UploadFile(model.UploadFile, AzureBlobContainer.Driver),
                Address = model.Address,
                Password = passwordHash,
                Salt = passwordSalt,
                Status = (int)DriverStatus.Active
            };

            await _unitOfWork.DriverRepository.Add(driver);
            await _unitOfWork.SaveChangesAsync();

            await _smsService.SendSMSByTwilio(model.Phone, password);

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
            var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(id));
            if (driver == null)
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
                Data = driver.AsDriverViewModel()
            };
        }

        public async Task<Response> GetList()
        {
            var list = await _unitOfWork.DriverRepository
                        .Query()
                        .Select(d => d.AsDriverViewModel())
                        .ToListAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetListSuccess,
                Data = list
            };
        }

        public Task<Response> Update(string id, UpdateDriverModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}