using System;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Data.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FBus.Business.BaseBusiness.Implements
{
    public class NotificationService : BaseService, INotificationService
    {
        public NotificationService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Response> SaveNotiToken(NotiTokenModel model, Role role)
        {
            switch (role)
            {
                case Role.Admin:
                    var admin = await _unitOfWork.AdminRepository.GetById(Guid.Parse(model.Id));
                    if (admin == null)
                    {
                        return new()
                        {
                            StatusCode = (int)StatusCode.NotFound,
                            Message = FBus.Business.BaseBusiness.Configuration.Message.NotFound
                        };
                    }
                    admin.NotifyToken = model.NotificationToken;
                    _unitOfWork.AdminRepository.Update(admin);
                    break;

                case Role.Driver:
                    var d = await _unitOfWork.DriverRepository.GetById(Guid.Parse(model.Id));
                    if (d == null)
                    {
                        return new()
                        {
                            StatusCode = (int)StatusCode.NotFound,
                            Message = FBus.Business.BaseBusiness.Configuration.Message.NotFound
                        };
                    }
                    d.NotifyToken = model.NotificationToken;
                    _unitOfWork.DriverRepository.Update(d);
                    break;

                case Role.Student:
                    var s = await _unitOfWork.StudentRepository.GetById(Guid.Parse(model.Id));
                    if (s == null)
                    {
                        return new()
                        {
                            StatusCode = (int)StatusCode.NotFound,
                            Message = FBus.Business.BaseBusiness.Configuration.Message.NotFound
                        };
                    }
                    s.NotifyToken = model.NotificationToken;
                    _unitOfWork.StudentRepository.Update(s);
                    break;

                default:
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = FBus.Business.BaseBusiness.Configuration.Message.CustomContent("Yêu cầu không hợp lệ! Vai trò không hợp lệ trong hệ thống"),
                    };
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = FBus.Business.BaseBusiness.Configuration.Message.UpdatedSuccess,
            };
        }

        public async Task<Response> SaveNotification(NoticationModel model, Role role)
        {
            switch (role)
            {
                case Role.Admin:
                    break;

                case Role.Driver:
                    await _unitOfWork.DriverNotificationRepository.Add(model.DriverNotification());
                    break;

                case Role.Student:
                    await _unitOfWork.StudentNotificationRepository.Add(model.StudentNotification());
                    break;

                default:
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = FBus.Business.BaseBusiness.Configuration.Message.CustomContent("Yêu cầu không hợp lệ! Vai trò không hợp lệ trong hệ thống"),
                    };
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = (int)StatusCode.Success,
                Message = FBus.Business.BaseBusiness.Configuration.Message.CreatedSuccess,
            };
        }

        public async Task SendNotification(string notiToken, string title, string content)
        {
            try
            {
                var message = new FirebaseAdmin.Messaging.Message()
                {
                    Token = notiToken,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = content
                    },
                };

                //Send message to device correspond 
                var response =
                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("ERROR: " + e);
            }
        }

        public async Task<Response> GetNotification(string id, Role role)
        {
            object data = new Array[0];
            switch (role)
            {
                case Role.Admin:
                    data = await _unitOfWork.ShiftRepository
                            .Query()
                            .Join(_unitOfWork.DriverRepository.Query(),
                                _ => _.DriverId,
                                driver => driver.DriverId,
                                (_, driver) =>
                                    new
                                    {
                                        ShiftId = _.ShiftId,
                                        DriverId = driver.DriverId,
                                        TripId = _.TripId,
                                        RequestTime = _.RequestTime,
                                        Content = _.Content,
                                        Type = _.Type,
                                        Status = _.Status,
                                        DriverPhoto = driver.PhotoUrl
                                    }
                            )
                            .OrderByDescending(x => x.RequestTime)
                            .ToListAsync();
                    break;

                case Role.Driver:
                    data = await _unitOfWork.DriverNotificationRepository
                            .Query()
                            .Where(d => d.DriverId == Guid.Parse(id))
                            .Select(d => new
                            {
                                Title = d.Title,
                                Content = d.Content,
                                Type = d.Type,
                                CreatedDate = d.CreateDate
                            })
                            .ToListAsync();
                    break;

                case Role.Student:
                    data = await _unitOfWork.StudentNotificationRepository
                            .Query()
                            .Where(s => s.StudentId == Guid.Parse(id))
                            .Select(s => new
                            {
                                Title = s.Title,
                                Content = s.Content,
                                Type = s.Type,
                                CreatedDate = s.CreateDate
                            })
                            .ToListAsync();
                    break;

                default:
                    return new()
                    {
                        StatusCode = (int)StatusCode.BadRequest,
                        Message = FBus.Business.BaseBusiness.Configuration.Message.CustomContent("Yêu cầu không hợp lệ! Vai trò không hợp lệ trong hệ thống"),
                    };
            }

            return new()
            {
                StatusCode = (int)StatusCode.Ok,
                Message = FBus.Business.BaseBusiness.Configuration.Message.GetListSuccess,
                Data = data,
            };
        }
    }
}