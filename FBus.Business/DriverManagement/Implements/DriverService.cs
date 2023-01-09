using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.DriverManagement.Interfaces;
using FBus.Business.DriverManagement.Models;
using FBus.Business.Student.Models;
using FBus.Business.TripManagement.Interfaces;
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
        ITripManagementService _tripService;
        public DriverService(IUnitOfWork unitOfWork, IConfiguration configuration, IAzureBlobService azureBlobService, ISMSService smsService, ITripManagementService tripManagementService) : base(unitOfWork)
        {
            _configuration = configuration;
            _azureBlobService = azureBlobService;
            _smsService = smsService;
            _tripService = tripManagementService;
        }

        private static string EncryptString(string plainText)
        {
            string key = "b14pa58l8aee4133bhce2ea2315b1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public  Response GenCode(string plainText)
        {
            var code = EncryptString(plainText);
            return new()
            {
                Data = code,
                StatusCode= (int) StatusCode.Ok
            };
        }

        public async Task<Response> Statistics(string id)
        {
            var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(id));
            DateTime fd = DateTime.UtcNow.AddHours(7).StartOfWeek(DayOfWeek.Monday);
            DateTime td = fd.AddDays(7);
            DateTime currentDate = DateTime.UtcNow.AddHours(7);
            var TripList = await _unitOfWork.TripRepository.Query().Where(x=> x.DriverId == driver.DriverId).ToListAsync();
            int tripCount = 0;
            int tripNotUse = 0;
            double distance = 0;
            foreach (var item in TripList)
            {
                var trip = await _unitOfWork.TripRepository.GetById(item.TripId);
                if (trip.Date >= fd && (trip.Date < currentDate || (trip.Date < currentDate &&  trip.TimeStart < currentDate.TimeOfDay))&& trip.Status== 4)
                {
                    tripCount++;
                    var route = await _unitOfWork.RouteRepository.GetById(trip.RouteId);
                    distance += Convert.ToDouble(route.Distance);
                }
                if((trip.Date > currentDate || (trip.Date > currentDate && trip.TimeStart > currentDate.TimeOfDay)) && trip.Date <= td)
                {
                    tripNotUse++;
                }
            }
            var result = new DriverStatisticsModel();
            result.Distance = distance;
            result.TripCount = tripCount;
            result.TripNotUseCount = tripNotUse;
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetDetailsSuccess,
                Data = result
            };

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

        public async Task<Response> Disable(string id)
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

            if (driver.Status == (int)DriverStatus.Assigned || driver.Status == (int)DriverStatus.Running)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Không thể xóa tài xế đang trong trạng thái hoạt động!")
                };
            }

            var isAssignedTrip = await _unitOfWork.TripRepository
                                .Query()
                                .Where(x => x.DriverId != null && x.DriverId.Value == Guid.Parse(id))
                                .Where(CompareTime())
                                .Where(x =>
                                        x.TimeStart.CompareTo(DateTime.UtcNow.AddHours(7).TimeOfDay) >= 0 ||
                                        (x.TimeStart.CompareTo(DateTime.UtcNow.AddHours(7).TimeOfDay) <= 0 && x.TimeEnd.CompareTo(DateTime.UtcNow.AddHours(7).TimeOfDay) >= 0)
                                    )
                                .FirstOrDefaultAsync();

            if (isAssignedTrip != null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Không thể xóa tài xế này! Tài xế đã được lập lịch chạy")
                };
            }

            driver.Status = (int)DriverStatus.Disable;
            _unitOfWork.DriverRepository.Update(driver);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess
            };
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
            DriverDetailModel result = new DriverDetailModel();
            result.Driver = driver.AsDriverViewModel();
            result.Trips = (List<TripViewModel>)_tripService.GetList(null, driver.DriverId).Result.Data;
            result.Rate = (float?)result.Trips.Average(x => x.Rate);


            List<string> feedbacks = new List<string>();
            if (result.Trips.Count > 0)
            {
                var studentTrips = await _unitOfWork.StudentTripRepository
                                                .Query()
                                                .Where(x => x.TripId == result.Trips[0].TripId)
                                                .ToListAsync();


                foreach (var item in studentTrips)
                {
                    feedbacks.Add(item.Feedback);
                }
            }
            result.Feedback = feedbacks;

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = Message.GetDetailsSuccess,
                Data = result
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

        public async Task<Response> TrackingLocation(TrackingLocationModel model)
        {
            var tr = await _unitOfWork.TrackingLocationRepository.Query().Where(x => x.DriverId == Guid.Parse(model.DriverId)).FirstOrDefaultAsync();
            if (tr != null)
            {
                _unitOfWork.TrackingLocationRepository.Remove(tr);
            }

            var entity = new TrackingLocation
            {
                TrackingLocationId = Guid.NewGuid(),
                DriverId = Guid.Parse(model.DriverId),
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.TrackingLocationRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.CustomContent("Lưu vị trí thành công")
            };
        }

        public async Task<Response> Update(string id, UpdateDriverModel model, bool isAdminRole = false)
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

            driver.FullName = UpdateTypeOfNullAbleObject<string>(driver.FullName, model.FullName);
            driver.PhotoUrl = await _azureBlobService.DeleteFile(model.DeleteFile, AzureBlobContainer.Driver, driver.PhotoUrl);
            driver.PhotoUrl += await _azureBlobService.UploadFile(model.UploadFile, AzureBlobContainer.Driver);
            driver.Address = UpdateTypeOfNullAbleObject<string>(driver.Address, model.Address);

            if (isAdminRole)
            {
                driver.Status = UpdateTypeOfNotNullAbleObject<int>(driver.Status, model.Status);
            }

            _unitOfWork.DriverRepository.Update(driver);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess
            };
        }

        private Expression<Func<Trip, bool>> CompareTime()
        {
            var day = DateTime.UtcNow.Day;
            var month = DateTime.UtcNow.Month;
            var year = DateTime.UtcNow.Year;

            DateTime now = new DateTime(year, month, day);

            return x => x.Date.CompareTo(now) >= 0;
        }
    }
}