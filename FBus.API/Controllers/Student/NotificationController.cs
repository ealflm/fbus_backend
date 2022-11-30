using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Student
{
    [ApiController]
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Student.Notification + "/{id}")]
        public async Task<IActionResult> GetNotification(string id)
        {
            return SendResponse(await _notificationService.GetNotification(id, Role.Student));
        }

        [HttpPost]
        [Route(ApiVer1Url.Student.NotificationToken)]
        public async Task<IActionResult> SaveNotiToken(NotiTokenModel model)
        {
            return SendResponse(await _notificationService.SaveNotiToken(model, Role.Student));
        }

        [HttpPost]
        [Route(ApiVer1Url.Student.Notification)]
        public async Task<IActionResult> SaveNotification(NoticationModel model)
        {
            return SendResponse(await _notificationService.SaveNotification(model, Role.Student));
        }

        [HttpPost]
        [Route(ApiVer1Url.Student.SendNotification)]
        public async Task<IActionResult> SendNotification(SendNotificationModel model)
        {
            await _notificationService.SendNotification(model.NotificationToken, model.Title, model.Content);
            return Ok();
        }
    }
}