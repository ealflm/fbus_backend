using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Business.TripManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notiService;
        private readonly ITripManagementService _tripService;

        public NotificationController(INotificationService notiService, ITripManagementService tripService)
        {
            _notiService = notiService;
            _tripService = tripService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.NotificationToken)]
        public async Task<IActionResult> SaveNotiToken([FromBody] NotiTokenModel model)
        {
            return SendResponse(await _notiService.SaveNotiToken(model, Role.Admin));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Notification)]
        public async Task<IActionResult> GetNotification()
        {
            return SendResponse(await _notiService.GetNotification("", Role.Admin));
        }

        [HttpPatch]
        [Route(ApiVer1Url.Admin.Notification + "/{id}")]
        public async Task<IActionResult> MakeReadRequest(string id)
        {
            return SendResponse(await _tripService.MakeReadRequest(id));
        }
    }
}