using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Implements;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.BaseBusiness.ViewModel;
using FBus.Business.StationManagement.Interfaces;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentTripManagement.Interfaces;
using FBus.Business.StudentTripManagement.SearchModel;
using FBus.Business.TripManagement.Interfaces;
using FBus.Data.Interfaces;
using FBus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FBus.Business.StudentTripManagement.Implements
{
    public class StudentTripManagementService : BaseService, IStudentTripManagementService
    {
        private ITripManagementService _tripManagementService;
        private IStationManagementService _stationManagementService;
        private IStudentService _studentManagementService;
        private static IConfiguration _configuration;
        private INotificationService _notificationService;
        public StudentTripManagementService(IUnitOfWork unitOfWork, ITripManagementService tripManagementService,
                                                IStationManagementService stationManagementService, IStudentService studentManagementService,
                                                IConfiguration configuration, INotificationService notificationService) : base(unitOfWork)
        {
            _tripManagementService = tripManagementService;
            _stationManagementService = stationManagementService;
            _studentManagementService = studentManagementService;
            _configuration = configuration;
            _notificationService = notificationService;
        }

        public async Task<Response> Create(StudentTripSearchModel model)
        {
            bool already = (await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId.Equals(model.TripId) && x.StudentId.Equals(model.StudentId)).FirstOrDefaultAsync()) != null;
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

            var student = await _unitOfWork.StudentRepository.GetById(model.StudentId);
            if (student != null)
            {
                // Save notification to db
                NoticationModel saveNoti = new NoticationModel
                {
                    EntityId = student.StudentId.ToString(),
                    Title = "Đặt vé xe buýt",
                    Content = "Đặt vé xe buýt thành công!",
                    Type = "Booking",
                };
                await _notificationService.SaveNotification(saveNoti, Role.Student);

                // Send notification to client
                await _notificationService.SendNotification(
                    student.NotifyToken,
                    "Đặt vé xe buýt",
                    "Bạn đã đặt vé thành công!"
                );

                // Xử lý cho phần tài xế
                // ... code 
            }

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

        public async Task<Response> GetList(Guid? id, DateTime? fromDate, DateTime? toDate, int? status)
        {
            var entities = await _unitOfWork.StudentTripRepository.Query()
                .Where(x => id == null || (id != null && x.StudentId.Equals(id)))
                .Where(x => status == null || (status != null && x.Status.Equals(status))).ToListAsync();
            var resultList = new List<StudentTripViewModel>();
            if (fromDate == null)
            {
                fromDate = DateTime.MinValue;
            }
            if (toDate == null)
            {
                toDate = DateTime.UtcNow.AddHours(7).AddDays(360);
            }
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                result.Station = (StationViewModel)(await _stationManagementService.Get(entity.StationId)).Data;
                result.Student = (StudentViewModel)(await _studentManagementService.GetByID(entity.StudentId.ToString())).Data;
                if(result.Trip.Date.CompareTo(fromDate)>=0 && result.Trip.Date.CompareTo(toDate) <= 0)
                {
                    resultList.Add(result);
                }
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList.OrderBy(x=>x.Trip.Date),
                Message = Message.GetListSuccess
            };
        }


        public async Task<Response> GetCurrent(Guid id)
        {
            var entities = await _unitOfWork.StudentTripRepository.Query()
                .Where(x => x.StudentId.Equals(id)).ToListAsync();
            var resultList = new List<StudentTripViewModel>();
            var fromDate = DateTime.UtcNow.AddHours(6);
            var toDate = DateTime.UtcNow.AddHours(7).AddDays(7);
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                result.Station = (StationViewModel)(await _stationManagementService.Get(entity.StationId)).Data;
                result.Student = (StudentViewModel)(await _studentManagementService.GetByID(entity.StudentId.ToString())).Data;
                if (result.Trip.Date.CompareTo(fromDate) >= 0 && result.Trip.Date.CompareTo(toDate) <= 0)
                {
                    resultList.Add(result);
                }
            }
            resultList.OrderBy(x => x.Trip.Date).OrderBy(x => x.Trip.TimeEnd);
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList.OrderBy(x=>x.Trip.Date).FirstOrDefault(),
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


        public async Task<Response> CheckIn(string qrCode)
        {
            var StudentTripID = new Guid(DecryptString(qrCode));
            var entity = await _unitOfWork.StudentTripRepository.GetById(StudentTripID);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.NotFound,
                    Message = Message.NotFound
                };
            }
            entity.Status = (int)StudentTripStatus.Passed;
            _unitOfWork.StudentTripRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var student = await _unitOfWork.StudentRepository.GetById(entity.StudentId);
            if (student != null)
            {
                // Save notification to db
                NoticationModel saveNoti = new NoticationModel
                {
                    EntityId = student.StudentId.ToString(),
                    Title = "Checkin",
                    Content = "Bạn vừa checkin thành công!",
                    Type = "Checkin",
                };
                await _notificationService.SaveNotification(saveNoti, Role.Student);

                // Send notification to client
                await _notificationService.SendNotification(
                    student.NotifyToken,
                    "Checkin",
                    "Bạn vừa checkin thành công!"
                );

                // Xử lý cho phần tài xế
                // ... code 
            }

            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = Message.UpdatedSuccess
            };
        }

        public static string DecryptString(string cipherText)
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
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
