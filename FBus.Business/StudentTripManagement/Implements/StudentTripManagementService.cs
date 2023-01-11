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
            var tripCheck = await _unitOfWork.TripRepository.GetById(model.TripId);
            var tripCount = await _unitOfWork.StudentTripRepository.Query().Where(x => x.TripId == tripCheck.TripId).CountAsync();
            var busVehicle = await _unitOfWork.BusRepository.GetById(tripCheck.BusVehicleId);
            if(tripCount >= busVehicle.Seat)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.Forbbiden,
                    Message = Message.ErrorExecution
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

            if (trip.Driver == null)
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BadRequest,
                    Message = Message.CustomContent("Không thể đặt vé với tuyến xe chưa có tài xế!")
                };
            }

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
                    Type = NotificationType.Booking,
                };
                await _notificationService.SaveNotification(saveNoti, Role.Student);

                // Send notification to client
                if (!string.IsNullOrEmpty(student.NotifyToken))
                {
                    await _notificationService.SendNotification(
                        student.NotifyToken,
                        "Đặt vé xe buýt",
                        "Bạn đã đặt vé thành công!"
                    );
                }

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

            // Get driver
            var driver = await _unitOfWork.TripRepository
                        .Query()
                        .Where(x => x.TripId == entity.TripId)
                        .Join(_unitOfWork.DriverRepository.Query(),
                            _ => _.DriverId.Value,
                            driver => driver.DriverId,
                            (_, driver) => new Driver
                            {
                                DriverId = driver.DriverId,
                                NotifyToken = driver.NotifyToken
                            }
                        )
                        .FirstOrDefaultAsync();

            string content = model.Feedback;
            if (model.Rate != 0)
            {
                content = $"Bạn vừa được đánh giá {model.Rate} sao. {model.Feedback}";
            }

            // Save notification to db
            NoticationModel saveNoti = new NoticationModel
            {
                EntityId = driver.DriverId.ToString(),
                Title = "Đánh giá chuyến đi",
                Content = content,
                Type = NotificationType.FeedBack,
            };
            await _notificationService.SaveNotification(saveNoti, Role.Student);

            // Send notification to client
            if (!string.IsNullOrEmpty(driver.NotifyToken))
            {
                await _notificationService.SendNotification(
                    driver.NotifyToken,
                    "Đánh giá chuyến đi",
                    content
                );

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
                if (result.Trip.Date.CompareTo(fromDate) >= 0 && result.Trip.Date.CompareTo(toDate) <= 0)
                {
                    resultList.Add(result);
                }
            }
            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Data = resultList.OrderBy(x => x.Trip.Date),
                Message = Message.GetListSuccess
            };
        }


        public async Task<Response> GetCurrent(Guid id)
        {
            var entities = await _unitOfWork.StudentTripRepository.Query()
                .Where(x => x.StudentId.Equals(id)).ToListAsync();
            var currentDate = DateTime.UtcNow.AddHours(7);
            var minTimeSpan = TimeSpan.MaxValue.TotalMinutes;
            StudentTripViewModel rs = null;
            foreach (var entity in entities)
            {
                var result = entity.AsViewModel();
                result.Trip = (TripViewModel)(await _tripManagementService.Get(entity.TripId)).Data;
                result.Station = (StationViewModel)(await _stationManagementService.Get(entity.StationId)).Data;
                result.Student = (StudentViewModel)(await _studentManagementService.GetByID(entity.StudentId.ToString())).Data;
                var timeSpan = Math.Abs( result.Trip.Date.AddMinutes(result.Trip.TimeStart.TotalMinutes).Subtract(currentDate).TotalMinutes);
                if (minTimeSpan> timeSpan)
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


        public async Task<Response> CheckIn(string qrCode, Guid studentID)
        {
            string s = DecryptString(qrCode);
            int count = int.Parse((s.Split('_'))[1]);
            var tripID = new Guid(s.Split('_')[0]);
            var trip = await _unitOfWork.TripRepository.GetById(tripID);
            var currentDate = DateTime.UtcNow.AddHours(7);
            var dateCheck = currentDate.Date;
            var timeCheck = currentDate.TimeOfDay;
            StudentTrip entity = await _unitOfWork.StudentTripRepository.Query().Where(y => y.TripId == trip.TripId && y.StudentId == studentID && y.Status==1).FirstOrDefaultAsync();
            var busVehicle = await _unitOfWork.BusRepository.GetById(trip.BusVehicleId);
            if (entity != null && trip.CurrentTicket< count && busVehicle.Seat >= count)
            {
                trip.CurrentTicket = count;
                _unitOfWork.TripRepository.Update(trip);
            }    
            else 
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
                    Type = NotificationType.Checkin,
                };
                await _notificationService.SaveNotification(saveNoti, Role.Student);

                // Send notification to client
                if (!string.IsNullOrEmpty(student.NotifyToken))
                {
                    await _notificationService.SendNotification(
                        student.NotifyToken,
                        "Checkin",
                        "Bạn vừa checkin thành công!"
                    );
                }

                // Xử lý cho phần tài xế
                
            }
            var driver = await _unitOfWork.DriverRepository.GetById(trip.DriverId.Value);
            if(driver != null)
            {
                NoticationModel saveNoti = new NoticationModel
                {
                    EntityId = driver.DriverId.ToString(),
                    Title = "Checkin",
                    Content = "Sinh viên "+ student.FullName+ " vừa lên xe",
                    Type = NotificationType.Checkin,
                };
                await _notificationService.SaveNotification(saveNoti, Role.Driver);

                // Send notification to client
                if (!string.IsNullOrEmpty(student.NotifyToken))
                {
                    await _notificationService.SendNotification(
                        driver.NotifyToken,
                        "Checkin",
                        "Sinh viên " + student.FullName + " vừa lên xe"
                    );
                }
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
