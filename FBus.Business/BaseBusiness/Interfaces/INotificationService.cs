using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FBus.Business.BaseBusiness.CommonModel;
using FBus.Business.BaseBusiness.Configuration;

namespace FBus.Business.BaseBusiness.Interfaces
{
    public interface INotificationService
    {
        Task<Response> SaveNotiToken(NotiTokenModel model, Role role);
        Task<Response> SaveNotification(NoticationModel model, Role role);
        Task SendNotification(string notiToken, string title, string content);
        Task<Response> GetNotification(string id, Role role);
    }

    public class NotiTokenModel
    {
        // Id can include: AdminId, StudentId, DriverId Some entity want to connect with Firebase cloud messaging
        [Required]
        public string Id { get; set; }

        public string NotificationToken { get; set; }
    }

    public class NoticationModel
    {
        public string EntityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
    }

    public class SendNotificationModel
    {
        public string NotificationToken { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}