using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Driver
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
        [Route(ApiVer1Url.Driver.Notification + "/{id}")]
        public async Task<IActionResult> GetNotification(string id)
        {
            return SendResponse(await _notificationService.GetNotification(id, Role.Driver));
        }

        [HttpPost]
        [Route(ApiVer1Url.Driver.NotificationToken)]
        public async Task<IActionResult> SaveNotiToken(NotiTokenModel model)
        {
            return SendResponse(await _notificationService.SaveNotiToken(model, Role.Driver));
        }

        [HttpPost]
        [Route(ApiVer1Url.Driver.Notification)]
        public async Task<IActionResult> SaveNotification(NoticationModel model)
        {
            return SendResponse(await _notificationService.SaveNotification(model, Role.Driver));
        }
    }
}